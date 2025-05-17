using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.FileSharing;

namespace ElsaMina.Commands.Ai.TextToSpeech;

[NamedCommand("speak", "aispeak", "ai-speak")]
public class SpeakCommand : Command
{
    private readonly IFileSharingService _fileSharingService;
    private readonly IClockService _clockService;
    private readonly IAiTextToSpeechProvider _textToSpeechProvider;

    public SpeakCommand(IFileSharingService fileSharingService, IClockService clockService,
        IAiTextToSpeechProvider textToSpeechProvider)
    {
        _fileSharingService = fileSharingService;
        _clockService = clockService;
        _textToSpeechProvider = textToSpeechProvider;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override Rank RequiredRank => Rank.Voiced;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var stream = await _textToSpeechProvider.GetTextToSpeechAudioStreamAsync(context.Target, cancellationToken);

        if (stream == null)
        {
            context.ReplyLocalizedMessage("speak_error");
            Log.Error("Failed to get TTS audio stream");
            return;
        }

        var fileName = $"speakcmd_{_clockService.CurrentUtcDateTime:yyyyMMdd_HHmmssfff}.mp3";
        var url = await _fileSharingService.CreateFileAsync(
            stream, fileName, "Speak command", "audio/mpeg", cancellationToken);

        if (string.IsNullOrEmpty(url))
        {
            context.ReplyLocalizedMessage("speak_error");
            Log.Error("Failed to upload audio file");
            return;
        }

        context.ReplyHtml($"""<audio src="{url}" controls></audio>""");
    }
}