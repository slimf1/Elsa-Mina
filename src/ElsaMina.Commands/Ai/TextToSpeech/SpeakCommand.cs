using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Ai.TextToSpeech;

[NamedCommand("speak", "aispeak", "ai-speak", "tts", "texttospeech", "text-to-speech")]
public class SpeakCommand : Command
{
    private readonly IAiTextToSpeechProvider _textToSpeechProvider;

    public SpeakCommand(IAiTextToSpeechProvider textToSpeechProvider)
    {
        _textToSpeechProvider = textToSpeechProvider;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override Rank RequiredRank => Rank.Voiced;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        string text;
        VoiceType voiceType;
        try
        {
            var args = context.Target.Split(";;");
            text = args[0];
            voiceType = Enum.Parse<VoiceType>(args[1].Trim(), ignoreCase: true);
        }
        catch (Exception)
        {
            text = context.Target;
            voiceType = VoiceType.Female;
        }

        var url = await _textToSpeechProvider.GetTextToSpeechAudioUrlAsync(text, voiceType, cancellationToken);

        if (string.IsNullOrEmpty(url))
        {
            context.ReplyLocalizedMessage("speak_error");
            Log.Error("Failed to upload audio file");
            return;
        }

        context.ReplyHtml($"""<audio src="{url}" controls aria-label="{text}"></audio>""");
    }
}