using JarvisWeb.Domain;
using JarvisWeb.Domain.Models;
using JarvisWeb.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Services
{
    public class EndOfDayNoteService(
        ILogger<EndOfDayNoteService> logger,
        JarvisWebDbContext context,
        ITranscriptionService transcriptionService,
        IConfiguration configuration) : BaseService<EndOfDayNote>(context, logger, context.EndOfDayNotes)
    {
        private readonly ITranscriptionService _transcriptionService = transcriptionService;
        private readonly IConfiguration _configuration = configuration;

        public async Task<EndOfDayNote?> CreateEndOfDayNoteFromAudioFile(Guid userId, IFormFile formFile)
        {
            var id = Guid.NewGuid();
            var audioDirectory = _configuration["AudioFiles:Path"];
            var path = Path.Combine(audioDirectory, $"{id}-{formFile.FileName}");
            using( var memoryStream = new MemoryStream() )
            {
                await formFile.OpenReadStream().CopyToAsync(memoryStream);
                await File.WriteAllBytesAsync(path, memoryStream.ToArray());
            }
            var transcription = await _transcriptionService.GetAudioFileTranscription(path);
            if (!transcription.IsSuccess) 
            {
                return null;
            }
            var endOfDayNote = new EndOfDayNote
            {
                Id = id,
                AudioFilePath = path,
                DateTime = DateTime.Now,
                UserId = userId,
                Note = transcription.Data
            };
            return await Create(endOfDayNote);
        }

        public async Task GenerateTranscript(EndOfDayNote endOfDayNote)
        {
            var file = endOfDayNote.AudioFilePath;
            var transcription = await _transcriptionService.GetAudioFileTranscription(file);
            if (!transcription.IsSuccess)
            {
                return;
            }
            endOfDayNote.Note = transcription.Data;
            await Update(endOfDayNote);

        }
    }
}
