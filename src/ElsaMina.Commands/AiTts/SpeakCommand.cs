using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Http;
using ElsaMina.FileSharing;

namespace ElsaMina.Commands.AiTts;

[NamedCommand("speak", "aispeak", "ai-speak")]
public class SpeakCommand : Command
{
    private const string VOICE_ID = "Qrl71rx6Yg8RvyPYRGCQ";
    private const string ELEVEN_LABS_TTS_API_URL = $"https://api.elevenlabs.io/v1/text-to-speech/{VOICE_ID}";

    private readonly IHttpService _httpService;
    private readonly IFileSharingService _fileSharingService;
    private readonly IClockService _clockService;
    private readonly IConfiguration _configuration;

    public SpeakCommand(IHttpService httpService,
        IFileSharingService fileSharingService, IClockService clockService, IConfiguration configuration)
    {
        _httpService = httpService;
        _fileSharingService = fileSharingService;
        _clockService = clockService;
        _configuration = configuration;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override Rank RequiredRank => Rank.Voiced;

    public override async Task Run(IContext context)
    {
        var key = _configuration.ElevenLabsApiKey;
        if (string.IsNullOrWhiteSpace(key))
        {
            Log.Error("Missing ElevenLabs API key");
            return;
        }

        var headers = new Dictionary<string, string>
        {
            ["xi-api-key"] = key
        };

        var text = context.Target;
        var dto = new ElevenLabsRequestDto
        {
            Text = text,
            ModelId = "eleven_multilingual_v2"
        };

        var stream = await _httpService.PostStreamAsync(ELEVEN_LABS_TTS_API_URL,
            dto, headers);

        var fileName = $"speakcmd_{_clockService.CurrentUtcDateTime:yyyyMMdd_HHmmssfff}.mp3";
        var url = await _fileSharingService.CreateFileAsync(
            stream, fileName, "Speak command", "audio/mpeg");

        if (!string.IsNullOrEmpty(url))
        {
            context.SendHtml($"""<audio src="{url}" controls></audio>""");
        }
    }
}