using System;
using System.Diagnostics;
using JarvisWeb.Services.Interfaces;
using JarvisWeb.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JarvisWeb.Services.Adapters.SadTalker;

public class SadTalkerService(
    ILogger<SadTalkerService> logger,
    IConfiguration configuration
    ) : IVideoGenerationService
{
    private readonly ILogger<SadTalkerService> _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    public async Task<ServiceResponseModel<string>> GenerateVideoFromText(string text, string fileName)
    {
        try
        {
            if (!fileName.EndsWith(".mp4"))
            {
                fileName += ".mp4";
            }
            text = text.Replace("\n", "");
            var generationCommand = _configuration["SadTalker:GenerationScript"];
            var filePath = _configuration["SadTalker:Path"];
            var command = $"{generationCommand} \"{text}\" \"{filePath}\" {fileName}";
            await BashUtilities.RunCommandWithBash(command, _logger);
            var fullPath = Path.Join(filePath, fileName);
            return new ServiceResponseModel<string>
            {
                IsSuccess = true,
                Data = fullPath
            };
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error in {service} {message}", "SadTalkerService", ex.Message);
            return new ServiceResponseModel<string>
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
