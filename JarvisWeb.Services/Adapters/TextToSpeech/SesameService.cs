using JarvisWeb.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Adapters.TextToSpeech;

public class SesameService(IConfiguration configuration, ILogger<SesameService> logger) : ITextToSpeechService, IDisposable
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<SesameService> _logger = logger;
    private readonly HttpClient _httpClient = new();
    private Process? _sesameProcess;
    private bool _disposed;

    public async Task Initialize()
    {
        if (_sesameProcess != null && !_sesameProcess.HasExited)
        {
            _logger.LogInformation("[SesameService] Sesame server is already running.");
            return;
        }
        _logger.LogInformation("[SesameService] Starting Sesame server...");
        try
        {
            // Start the Sesame server as a subprocess
            _sesameProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Environment.OSVersion.Platform == PlatformID.Win32NT ? "cmd.exe" : "/bin/bash",
                    Arguments = Environment.OSVersion.Platform == PlatformID.Win32NT 
                        ? $"/c python path\\to\\sesame_server.py" 
                        : $"-c \"/home/josh/Documents/source/JarvisWeb/JarvisWeb.Services/Adapters/TextToSpeech/start_sesame.sh\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            _sesameProcess.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    _logger.LogInformation("[SesameService] Sesame server output: {output}", args.Data);
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

            _logger.LogInformation("[SesameService] Sesame server started with PID: {pid}", _sesameProcess.Id);

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
                    _logger.LogInformation("[SesameService] Warming up Sesame server. Attempt {attempt}", attempts + 1);
                    var response = await ConvertTextToSpeech("Warm-up text", "default", "mp3", "/dev/null");
                    if (response.IsSuccess)
                    {
                        isWarmedUp = true;
                        _logger.LogInformation("[SesameService] Sesame server warmed up successfully.");
                    }
                    else
                    {
                        _logger.LogWarning("[SesameService] Warm-up attempt failed: {errorMessage}", response.ErrorMessage);
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
                _logger.LogError("[SesameService] Failed to warm up Sesame server after 3 attempts.");
                throw new Exception("Failed to warm up Sesame server.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SesameService] Failed to start Sesame server.");
            throw;
        }
    }

    public async Task<ServiceResponseModel<string>> ConvertTextToSpeech(string text, string voiceId, string outputFormat, string audioFilePath)
    {
        try
        {
            if (_sesameProcess == null)
            {
                await Initialize();
            }
            // Construct the request URL
            var requestUrl = $"http://localhost:5000/speak?sentence={Uri.EscapeDataString(text)}";

            // Send the request to the Sesame server
            _logger.LogInformation("[SesameService] Sending request to Sesame server: {url}", requestUrl);
            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("[SesameService] Failed to retrieve audio. Status code: {statusCode}", response.StatusCode);
                return new ServiceResponseModel<string>
                {
                    IsSuccess = false,
                    ErrorMessage = $"Failed to retrieve audio. Status code: {response.StatusCode}"
                };
            }

            // Save the audio file to the specified path
            await using var fileStream = new FileStream(audioFilePath, FileMode.Create, FileAccess.Write);
            await response.Content.CopyToAsync(fileStream);

            _logger.LogInformation("[SesameService] Audio file saved successfully at: {path}", audioFilePath);
            return new ServiceResponseModel<string>
            {
                IsSuccess = true,
                Data = audioFilePath
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SesameService] Error during ConvertTextToSpeech.");
            return new ServiceResponseModel<string>
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
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
        await _httpClient.PostAsync("http://localhost:5000/shutdown", null);
        // Dispose managed resources
        if (_sesameProcess != null && !_sesameProcess.HasExited)
        {
            _logger.LogInformation("[SesameService] Stopping Sesame server with PID: {pid}", _sesameProcess.Id);
            _sesameProcess.Kill();
            _sesameProcess.Dispose();
            _sesameProcess = null;
        }

        _httpClient.Dispose();
        _logger.LogInformation("[SesameService] HttpClient disposed.");
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

    ~SesameService()
    {
        Dispose(disposing: false);
    }
}
