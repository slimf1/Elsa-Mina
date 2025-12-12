using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Ai.LanguageModel.Google;

public class GeminiLanguageModelProvider : ILanguageModelProvider
{
    private const string BASE_URL =
        "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

    private readonly IConfiguration _configuration;
    private readonly IHttpService _httpService;

    public GeminiLanguageModelProvider(IConfiguration configuration, IHttpService httpService)
    {
        _configuration = configuration;
        _httpService = httpService;
    }

    public async Task<string> AskLanguageModelAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration.GeminiApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            Log.Error("Missing Gemini API key");
            return null;
        }

        var headers = new Dictionary<string, string>
        {
            ["x-goog-api-key"] = apiKey
        };

        GeminiRequestDto requestDto = new();
        requestDto.Contents ??= [];
        requestDto.Contents.Add(new Content
        {
            Role = "user",
            Parts = [new ContentPart { Text = prompt }]
        });

        Log.Information("Making request to Gemini with prompt: {0}", prompt);
        var response = await _httpService.PostJsonAsync<GeminiRequestDto, GeminiResponseDto>(BASE_URL, requestDto,
            cancellationToken: cancellationToken, headers: headers);

        return response.Data.Candidates.First().Content.Parts[0].Text;
    }

    public async Task<string> AskLanguageModelAsync(LanguageModelRequest request,
        CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration.GeminiApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            Log.Error("Missing Gemini API key");
            return null;
        }


        GeminiRequestDto requestDto = new();
        if (!string.IsNullOrEmpty(request.SystemPrompt))
        {
            requestDto.SystemInstruction.Parts[0].Text = request.SystemPrompt;
        }

        foreach (var message in request.InputConversation ?? [])
        {
            requestDto.Contents ??= [];
            var role = message.Role switch
            {
                MessageRole.Agent => "model",
                MessageRole.User => "user",
                _ => "user"
            };
            requestDto.Contents.Add(new Content
            {
                Role = role,
                Parts = [new ContentPart { Text = message.Content }]
            });
        }

        var response =
            await _httpService.PostJsonAsync<GeminiRequestDto, GeminiResponseDto>(BASE_URL, requestDto,
                cancellationToken: cancellationToken);

        return response.Data.Candidates.First().Content.Parts[0].Text;
    }
}