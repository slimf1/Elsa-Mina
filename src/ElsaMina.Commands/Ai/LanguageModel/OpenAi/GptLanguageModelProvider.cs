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
        
        Log.Information("Making request to OpenAI's GPT with prompt: {0}", prompt);
        var response = await _httpService.PostJsonAsync<GptRequestDto, GptResponseDto>(
            "https://api.openai.com/v1/responses",
            dto,
            headers: headers,
            cancellationToken: cancellationToken);

        return response?.Data?.Output?.FirstOrDefault()?.Content?.FirstOrDefault()?.Text;
    }
    
    public async Task<string> AskLanguageModelAsync(LanguageModelRequest request, CancellationToken cancellationToken = default)
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

        var inputMessages = new List<object>();
        if (!string.IsNullOrWhiteSpace(request.SystemPrompt))
        {
            inputMessages.Add(new
            {
                role = "system",
                content = request.SystemPrompt
            });
        }

        if (request.InputConversation != null)
        {
            foreach (var msg in request.InputConversation)
            {
                if (msg == null)
                {
                    continue;
                }

                var role = msg.Role switch
                {
                    MessageRole.Agent => "assistant",
                    MessageRole.User => "user",
                    _ => "user"
                };
                inputMessages.Add(new
                {
                    role = role,
                    content = msg.Content
                });
            }
        }

        var dto = new
        {
            Model = "gpt-4.1-nano",
            Input = inputMessages
        };

        var response = await _httpService.PostJsonAsync<object, GptResponseDto>(
            "https://api.openai.com/v1/responses",
            dto,
            headers: headers,
            cancellationToken: cancellationToken);

        return response?
            .Data?
            .Output?
            .FirstOrDefault()?
            .Content?
            .FirstOrDefault()?
            .Text;
    }

}