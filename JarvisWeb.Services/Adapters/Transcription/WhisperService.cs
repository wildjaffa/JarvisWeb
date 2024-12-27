using JarvisWeb.Services.Interfaces;
using JarvisWeb.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Adapters.Transcription
{
    public class WhisperService: ITranscriptionService
    {
        public async Task<ServiceResponseModel<string>> GetAudioFileTranscription(string audioFilePath)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return new ServiceResponseModel<string>
                {
                    Data = ex.Message,
                    IsSuccess = false
                };
            }
        }
    }
}
