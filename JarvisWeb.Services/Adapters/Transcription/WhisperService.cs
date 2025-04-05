using JarvisWeb.Services.Interfaces;
using JarvisWeb.Services.Models;
using JarvisWeb.Services.Models.Whisper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Adapters.Transcription
{
    public class WhisperService(IConfiguration configuration, ILogger<WhisperService> logger): ITranscriptionService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<WhisperService> _logger = logger;
        public async Task<ServiceResponseModel<string>> GetAudioFileTranscription(string audioFilePath)
        {
            try
            {
                var tempPath = System.IO.Path.GetTempPath();
                var scriptPath = _configuration["Whisper:GenerationScript"];
                if (!File.Exists(audioFilePath))
                {
                    return new ServiceResponseModel<string>
                    {
                        IsSuccess = false,
                        ErrorMessage = "File does not exist"
                    };
                }
                var fileName = Path.GetFileName(audioFilePath);
                var command = $"{scriptPath} \"{audioFilePath}\" \"{tempPath}\"";
                await BashUtilities.RunCommandWithBash(command, _logger);
                var jsonResultPath = Path.Combine(tempPath, fileName+ ".json");
                if (!File.Exists(jsonResultPath))
                {
                    return new ServiceResponseModel<string>
                    {
                        IsSuccess = false,
                        ErrorMessage = "There was an issue generating the transcription"
                    };
                }
                var resultJson = File.ReadAllText(jsonResultPath);
                var parsed = JsonConvert.DeserializeObject<TranscriptionResult>(resultJson);

                return new ServiceResponseModel<string>
                {
                    IsSuccess = true,
                    Data = parsed.Text
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseModel<string>
                {
                    ErrorMessage = ex.Message,
                    IsSuccess = false
                };
            }
        }
    }
}
