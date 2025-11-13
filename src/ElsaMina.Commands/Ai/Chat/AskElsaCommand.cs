using ElsaMina.Commands.Ai.LanguageModel;
using ElsaMina.Commands.Ai.TextToSpeech;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Ai.Chat;

[NamedCommand("ask-elsa", "ask", "ai", "aichat", "ai-chat", "askaudio", "askelsaaudio", "ask-elsa-audio",
    "ai-chat-audio", "aiaudio", "ai-audio")]
public class AskElsaCommand : Command
{
    private readonly IConfiguration _configuration;
    private readonly IResourcesService _resourcesService;
    private readonly ILanguageModelProvider _languageModelProvider;
    private readonly IAiTextToSpeechProvider _textToSpeechProvider;

    public AskElsaCommand(IConfiguration configuration,
        IResourcesService resourcesService,
        ILanguageModelProvider languageModelProvider,
        IAiTextToSpeechProvider textToSpeechProvider)
    {
        _configuration = configuration;
        _resourcesService = resourcesService;
        _languageModelProvider = languageModelProvider;
        _textToSpeechProvider = textToSpeechProvider;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var withAudio = context.Command.Contains("audio");
        var room = context.Room;
        var prompt = string.Format(
            _resourcesService.GetString("ask_prompt", context.Culture),
            context.Target,
            context.Sender.Name,
            _configuration.Name,
            room.Name,
            string.Join(", ", room.LastMessages.Select(pair => $"{pair.Item1}: {pair.Item2}")));

        var response = await _languageModelProvider.AskLanguageModelAsync(prompt, cancellationToken);
        if (response == null)
        {
            context.ReplyLocalizedMessage("ask_error");
            return;
        }

        if (withAudio)
        {
            var url = await _textToSpeechProvider.GetTextToSpeechAudioUrlAsync(response, VoiceType.Female,
                cancellationToken);
            if (string.IsNullOrEmpty(url))
            {
                context.ReplyLocalizedMessage("ask_error");
                Log.Error("Failed to upload audio file");
                return;
            }

            context.SendHtmlIn($"""<audio src="{url}" controls aria-label="{response}"></audio>""");
            return;
        }

        context.Reply(response);
    }
}