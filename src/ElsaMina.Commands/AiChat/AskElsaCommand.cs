using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.AiChat;

[NamedCommand("ask-elsa", "ask", "ai", "aichat", "ai-chat")]
public class AskElsaCommand : Command
{
    private const string MISTRAL_AUTOCOMPLETE_URL = "https://api.mistral.ai/v1/chat/completions";
    private const string DEFAULT_MODEL = "mistral-large-latest";
    private const string DEFAULT_ROLE = "user";

    private const string PROMPT =
        "Tu es un robot dans un salon français sur un simulateur de combat Pokémon en ligne. Tu dois répondre au message suivant : \"{0}\", venant de l'utilisateur \"{1}\". La réponse ne doit pas dépasser 300 caractères et tu dois n'envoyer qu'une ligne.";
    
    private readonly IHttpService _httpService;
    private readonly IConfigurationManager _configurationManager;

    public AskElsaCommand(IHttpService httpService,
        IConfigurationManager configurationManager)
    {
        _httpService = httpService;
        _configurationManager = configurationManager;
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
                    Role = DEFAULT_ROLE,
                    Content = string.Format(PROMPT, context.Target, context.Sender.Name)
                }
            ]
        };
        var response = await _httpService.PostJson<MistralRequestDto, MistralResponseDto>(
            MISTRAL_AUTOCOMPLETE_URL,
            dto,
            headers: headers
        );

        var choice = response.Data?.Choices?.FirstOrDefault();
        if (choice == null)
        {
            context.Reply("ask_error");
            return;
        }
        
        context.Reply(choice.Message.Content);
    }
}