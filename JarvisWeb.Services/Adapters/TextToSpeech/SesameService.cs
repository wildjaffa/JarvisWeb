using System.Collections.Concurrent;
using System.Diagnostics;
using JarvisWeb.Services.Models;
using JarvisWeb.Services.Models.Sesame;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace JarvisWeb.Services.Adapters.TextToSpeech;

public class SesameService(IConfiguration configuration, ILogger<SesameService> logger)
    : ITextToSpeechService,
        IDisposable
{
    private readonly ConcurrentQueue<(
        string Text,
        TaskCompletionSource<ServiceResponseModel<string>> Tcs
    )> _queue = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _isProcessingQueue = false;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<SesameService> _logger = logger;
    private readonly RestClient _restClient = new("http://localhost:5000");
    private Process? _sesameProcess;
    private bool _sesameRunningSeperately = false;
    private bool _disposed;

    public async Task Initialize()
    {
        if ((_sesameProcess != null && !_sesameProcess.HasExited) || _sesameRunningSeperately)
        {
            _logger.LogInformation("[SesameService] Sesame server is already running.");
            return;
        }
        _logger.LogInformation("[SesameService] Starting Sesame server...");
        try
        {
            // Check if the server is already running
            try
            {
                var result = await ConvertTextToSpeech(
                    "Warm-up text",
                    "default",
                    "mp3",
                    "/dev/null",
                    false
                );
                if (result.IsSuccess)
                {
                    _sesameRunningSeperately = true;
                    _logger.LogInformation("[SesameService] Sesame server is already running.");
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Server is not running, proceed to start it
            }

            // Start the Sesame server as a subprocess
            _sesameProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName =
                        Environment.OSVersion.Platform == PlatformID.Win32NT
                            ? "cmd.exe"
                            : "/bin/bash",
                    Arguments =
                        Environment.OSVersion.Platform == PlatformID.Win32NT
                            ? $"/c python path\\to\\sesame_server.py"
                            : $"-c \"/home/josh/Documents/source/JarvisWeb/JarvisWeb.Services/Adapters/TextToSpeech/start_sesame.sh\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };

            _sesameProcess.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    _logger.LogInformation(
                        "[SesameService] Sesame server output: {output}",
                        args.Data
                    );
                }
            };

            _sesameProcess.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    _logger.LogError("[SesameService] Sesame server error: {error}", args.Data);
                }
            };

            _sesameProcess.Start();
            _sesameProcess.BeginOutputReadLine();
            _sesameProcess.BeginErrorReadLine();

            _logger.LogInformation(
                "[SesameService] Sesame server started with PID: {pid}",
                _sesameProcess.Id
            );

            // Wait for the server to start (you may want to implement a more robust check)
            await Task.Delay(10000); // Adjust the delay as needed
            if (_sesameProcess.HasExited)
            {
                _logger.LogError("[SesameService] Sesame server has exited unexpectedly.");
                throw new Exception("Sesame server has exited unexpectedly.");
            }
            _logger.LogInformation("[SesameService] Sesame server is running.");
            var attempts = 0;
            var isWarmedUp = false;

            while (attempts < 3 && !isWarmedUp)
            {
                try
                {
                    _logger.LogInformation(
                        "[SesameService] Warming up Sesame server. Attempt {attempt}",
                        attempts + 1
                    );
                    var response = await ConvertTextToSpeech(
                        "Warm-up text",
                        "default",
                        "wav",
                        "/dev/null"
                    );
                    if (response.IsSuccess)
                    {
                        isWarmedUp = true;
                        _logger.LogInformation(
                            "[SesameService] Sesame server warmed up successfully."
                        );
                    }
                    else
                    {
                        _logger.LogWarning(
                            "[SesameService] Warm-up attempt failed: {errorMessage}",
                            response.ErrorMessage
                        );
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[SesameService] Exception during warm-up attempt.");
                }

                if (!isWarmedUp)
                {
                    attempts++;
                    await Task.Delay(5000);
                }
            }

            if (!isWarmedUp)
            {
                _logger.LogError(
                    "[SesameService] Failed to warm up Sesame server after 3 attempts."
                );
                throw new Exception("Failed to warm up Sesame server.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SesameService] Failed to start Sesame server.");
            throw;
        }
    }

    public async Task<ServiceResponseModel<string>> ConvertTextToSpeech(
        string text,
        string voiceId,
        string outputFormat,
        string audioFilePath,
        bool? startIfNotRunning = true
    )
    {
        var tcs = new TaskCompletionSource<ServiceResponseModel<string>>();
        _queue.Enqueue((text, tcs));

        // Start processing the queue if not already running
        if (!_isProcessingQueue)
        {
            _isProcessingQueue = true;
            _ = ProcessQueueAsync();
        }

        return await tcs.Task;
    }

    private async Task ProcessQueueAsync()
    {
        while (_queue.TryDequeue(out var item))
        {
            var (text, tcs) = item;

            try
            {
                await _semaphore.WaitAsync();

                // Process the request
                var result = await ProcessTextToSpeechAsync(text);
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        _isProcessingQueue = false;
    }

    private async Task<ServiceResponseModel<string>> ProcessTextToSpeechAsync(string inputText)
    {
        var generationText = SanitizeStringForGeneration(inputText);
        var request = new RestRequest("speak", Method.Get);
        request.AddParameter("sentence", generationText);
        _logger.LogInformation("Generating Audio for {text}", generationText);
        var response = await _restClient.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            return new ServiceResponseModel<string>
            {
                IsSuccess = false,
                ErrorMessage = $"Failed to retrieve audio. Status code: {response.StatusCode}",
            };
        }

        var fileLocation = JsonConvert.DeserializeObject<GenerateAudioResponse>(response.Content!);
        return new ServiceResponseModel<string>
        {
            IsSuccess = true,
            Data = fileLocation.AudioFilePath,
        };
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async Task TearDown()
    {
        if (_sesameProcess == null || _sesameProcess.HasExited)
        {
            _logger.LogInformation("[SesameService] Sesame server is not running.");
            return;
        }
        var shutdownRequest = new RestRequest("shutdown", Method.Post);
        await _restClient.ExecuteAsync(shutdownRequest);
        // Dispose managed resources
        if (_sesameProcess != null && !_sesameProcess.HasExited)
        {
            _logger.LogInformation(
                "[SesameService] Stopping Sesame server with PID: {pid}",
                _sesameProcess.Id
            );
            _sesameProcess.Kill();
            _sesameProcess.Dispose();
            _sesameProcess = null;
        }

        _logger.LogInformation("[SesameService] HttpClient disposed.");
        _sesameRunningSeperately = false;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            TearDown().Wait();
        }

        // Free unmanaged resources (if any)

        _disposed = true;
    }

    public Task<ServiceResponseModel<string>> ConvertTextToSpeech(
        string text,
        string voiceId,
        string outputFormat,
        string audioFilePath
    )
    {
        return ConvertTextToSpeech(text, voiceId, outputFormat, audioFilePath, false);
    }

    ~SesameService()
    {
        Dispose(disposing: false);
    }

    private string SanitizeStringForGeneration(string text)
    {
        var returnText = text.Replace("\n", " ").Replace("\r", " ");
        returnText = new string(
            [.. returnText.Where(c => char.IsLetterOrDigit(c) || ",!?.' ".Contains(c))]
        );
        while (returnText.Contains("  "))
            returnText = returnText.Replace("  ", " ");
        _logger.LogInformation(
            "Sending the following text to be converted to speech: {text}",
            returnText
        );
        return returnText;
    }
}
