using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Resources;

namespace ElsaMina.Commands.AiChat;

[NamedCommand("ask-elsa", "ask", "ai", "aichat", "ai-chat")]
public class AskElsaCommand : Command
{
    private const string MISTRAL_AUTOCOMPLETE_API_URL = "https://api.mistral.ai/v1/chat/completions";
    private const string DEFAULT_MODEL = "mistral-large-latest";
    private const string DEFAULT_ROLE = "user";

    private readonly IHttpService _httpService;
    private readonly IConfigurationManager _configurationManager;
    private readonly IResourcesService _resourcesService;

    public AskElsaCommand(IHttpService httpService,
        IConfigurationManager configurationManager,
        IResourcesService resourcesService)
    {
        _httpService = httpService;
        _configurationManager = configurationManager;
        _resourcesService = resourcesService;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override async Task Run(IContext context)
    {
        var key = _configurationManager.Configuration.MistralApiKey;
        if (string.IsNullOrWhiteSpace(key))
        {
            Logger.Error("Missing Mistral API key");
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
                        _configurationManager.Configuration.Name,
                        room.Name,
                        string.Join(", ", room.LastMessages.Select(pair => $"{pair.Item1}: {pair.Item2}")))
                }
            ]
        };
        var response = await _httpService.PostJson<MistralRequestDto, MistralResponseDto>(
            MISTRAL_AUTOCOMPLETE_API_URL,
            dto,
            headers: headers
        );

        var choice = response?.Data?.Choices?.FirstOrDefault();
        if (choice == null)
        {
            context.ReplyLocalizedMessage("ask_error");
            return;
        }

        context.Reply(choice.Message.Content);
    }
}