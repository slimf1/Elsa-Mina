using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.AiChat;

[NamedCommand("ask-elsa", "ask", "ai", "aichat", "ai-chat")]
public class AskElsaCommand : Command
{
    private const string MISTRAL_AUTOCOMPLETE_API_URL = "https://api.mistral.ai/v1/chat/completions";
    private const string DEFAULT_MODEL = "mistral-large-latest";
    private const string DEFAULT_ROLE = "user";

    private readonly IHttpService _httpService;
    private readonly IConfiguration _configuration;
    private readonly IResourcesService _resourcesService;

    public AskElsaCommand(IHttpService httpService,
        IConfiguration configuration,
        IResourcesService resourcesService)
    {
        _httpService = httpService;
        _configuration = configuration;
        _resourcesService = resourcesService;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var key = _configuration.MistralApiKey;
        if (string.IsNullOrWhiteSpace(key))
        {
            Log.Error("Missing Mistral API key");
            return;
        }

        var room = context.Room;
        var headers = new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {key}"
        };
        var prompt = _resourcesService.GetString("ask_prompt", context.Culture);
        var dto = new MistralRequestDto
        {
            Model = DEFAULT_MODEL,
            Messages =
            [
                new MistralRequestMessageDto
                {
                    Role = DEFAULT_ROLE,
                    Content = string.Format(
                        prompt,
                        context.Target,
                        context.Sender.Name,
                        _configuration.Name,
                        room.Name,
                        string.Join(", ", room.LastMessages.Select(pair => $"{pair.Item1}: {pair.Item2}")))
                }
            ]
        };
        var response = await _httpService.PostJsonAsync<MistralRequestDto, MistralResponseDto>(
            MISTRAL_AUTOCOMPLETE_API_URL,
            dto,
            headers: headers,
            cancellationToken: cancellationToken);

        var choice = response?.Data?.Choices?.FirstOrDefault();
        if (choice == null)
        {
            context.ReplyLocalizedMessage("ask_error");
            return;
        }

        context.Reply(choice.Message.Content);
    }
}