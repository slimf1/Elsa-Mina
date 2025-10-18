using ElsaMina.Commands.Ai.Chat;
using ElsaMina.Core;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Ai.LanguageModel;

public class MistralLanguageModelProvider : ILanguageModelProvider
{
    private const string MISTRAL_AUTOCOMPLETE_API_URL = "https://api.mistral.ai/v1/chat/completions";
    private const string DEFAULT_MODEL = "mistral-medium-latest";
    private const string USER_ROLE = "user";

    private readonly IHttpService _httpService;
    private readonly IConfiguration _configuration;

    public MistralLanguageModelProvider(IHttpService httpService, IConfiguration configuration)
    {
        _httpService = httpService;
        _configuration = configuration;
    }

    public async Task<string> AskLanguageModelAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var key = _configuration.MistralApiKey;
        if (string.IsNullOrWhiteSpace(key))
        {
            Log.Error("Missing Mistral API key");
            return null;
        }

        var headers = new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {key}"
        };

        var dto = new MistralRequestDto
        {
            Model = DEFAULT_MODEL,
            Messages =
            [
                new MistralRequestMessageDto
                {
                    Role = USER_ROLE,
                    Content = prompt
                }
            ]
        };

        var response = await _httpService.PostJsonAsync<MistralRequestDto, MistralResponseDto>(
            MISTRAL_AUTOCOMPLETE_API_URL,
            dto,
            headers: headers,
            cancellationToken: cancellationToken);

        var choice = response?.Data?.Choices?.FirstOrDefault();
        return choice?.Message.Content;
    }
}