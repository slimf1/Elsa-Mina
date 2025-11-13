using ElsaMina.Core;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Ai.LanguageModel.OpenAi;

public class GptLanguageModelProvider : ILanguageModelProvider
{
    private readonly IHttpService _httpService;
    private readonly IConfiguration _configuration;

    public GptLanguageModelProvider(IHttpService httpService, IConfiguration configuration)
    {
        _httpService = httpService;
        _configuration = configuration;
    }

    public async Task<string> AskLanguageModelAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration.ChatGptApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Log.Error("Missing ChatGPT API key");
            return null;
        }
        
        var headers = new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {apiKey}"
        };
        
        var dto = new GptRequestDto
        {
            Model = "gpt-4.1-nano",
            Input = prompt
        };
        
        var response = await _httpService.PostJsonAsync<GptRequestDto, GptResponseDto>(
            "https://api.openai.com/v1/responses",
            dto,
            headers: headers,
            cancellationToken: cancellationToken);

        return response?.Data?.Output?.FirstOrDefault()?.Content?.FirstOrDefault()?.Text;
    }
}