using ElsaMina.Core;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Ai.TextToSpeech;

public class ElevenLabsAiTextToSpeechProvider : IAiTextToSpeechProvider
{
    private const string VOICE_ID = "Qrl71rx6Yg8RvyPYRGCQ";
    private const string ELEVEN_LABS_TTS_API_URL = $"https://api.elevenlabs.io/v1/text-to-speech/{VOICE_ID}";
    
    private readonly IConfiguration _configuration;
    private readonly IHttpService _httpService;

    public ElevenLabsAiTextToSpeechProvider(IConfiguration configuration, IHttpService httpService)
    {
        _configuration = configuration;
        _httpService = httpService;
    }

    public Task<Stream> GetTextToSpeechAudioStreamAsync(string text, CancellationToken cancellationToken = default)
    {
        var key = _configuration.ElevenLabsApiKey;
        if (string.IsNullOrWhiteSpace(key))
        {
            Log.Error("Missing Eleven Labs API key");
            return Task.FromResult<Stream>(null);
        }

        var headers = new Dictionary<string, string>
        {
            ["xi-api-key"] = key
        };

        var dto = new ElevenLabsRequestDto
        {
            Text = text,
            ModelId = "eleven_multilingual_v1"
        };

        return _httpService.PostStreamAsync(
            ELEVEN_LABS_TTS_API_URL,
            dto,
            headers: headers,
            cancellationToken: cancellationToken);
    }
}