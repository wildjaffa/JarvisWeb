using System.Text;
using JarvisWeb.Services.Adapters.LLM;
using JarvisWeb.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace JarvisWeb.Services.Services;

public class ConversationService(
    ILLMService llmService,
    ITextToSpeechService textToSpeechService,
    ILogger<ConversationService> logger
)
{
    private readonly ILLMService _llmService = llmService;
    private readonly ITextToSpeechService _textToSpeechService = textToSpeechService;
    private readonly ILogger<ConversationService> _logger = logger;

    private readonly string[] _endOfSentencePunctuation = [".", "!", "?"];

    public async IAsyncEnumerable<ConversationPart> GetResponseAsync(string prompt)
    {
        var textBuilder = new StringBuilder();
        var response = _llmService.StreamLLMCompletionAsync(prompt);

        await foreach (var part in response)
        {
            textBuilder.Append(part.Message.Content);

            // Check if the current part ends with sentence-ending punctuation
            if (_endOfSentencePunctuation.Contains(part.Message.Content.Trim()))
            {
                var text = textBuilder.ToString();
                textBuilder.Clear();

                // Generate audio for the completed sentence
                var audio = await _textToSpeechService.ConvertTextToSpeech(text, "", "", "");

                // Yield the result as soon as both text and audio are ready
                yield return new ConversationPart { Text = text, Audio = audio.Data };
            }
        }

        // Handle any remaining text that wasn't followed by punctuation
        if (textBuilder.Length > 0)
        {
            var text = textBuilder.ToString();
            var audio = await _textToSpeechService.ConvertTextToSpeech(text, "", "", "");
            yield return new ConversationPart { Text = text, Audio = audio.Data };
        }
    }
}

public class ConversationPart
{
    public string Text { get; set; }
    public string Audio { get; set; }
}
