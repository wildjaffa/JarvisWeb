using JarvisWeb.Domain;
using JarvisWeb.Domain.Models;
using JarvisWeb.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        ITranscriptionService transcriptionService) : BaseService<EndOfDayNote>(context, logger, context.EndOfDayNotes)
    {
        private readonly ITranscriptionService _transcriptionService = transcriptionService;

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
