using System;
using JarvisWeb.Services.Models;

namespace JarvisWeb.Services.Interfaces;

public interface IVideoGenerationService
{
    public Task<ServiceResponseModel<string>> GenerateVideoFromText(string text, string resultPath);

}
