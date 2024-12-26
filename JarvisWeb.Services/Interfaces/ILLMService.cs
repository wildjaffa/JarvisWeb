using JarvisWeb.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Interfaces
{
    public interface ILLMService
    {
        Task<ServiceResponseModel<LLMCompletion>> GetLLMCompletionAsync(string prompt);
    }
}
