using ElsaMina.Commands.Ai.LanguageModel;
using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Ai.Chat;

[NamedCommand("ask-elsa", "ask", "ai", "aichat", "ai-chat")]
public class AskElsaCommand : Command
{
    private readonly IConfiguration _configuration;
    private readonly IResourcesService _resourcesService;
    private readonly ILanguageModelProvider _languageModelProvider;

    public AskElsaCommand(IConfiguration configuration,
        IResourcesService resourcesService,
        ILanguageModelProvider languageModelProvider)
    {
        _configuration = configuration;
        _resourcesService = resourcesService;
        _languageModelProvider = languageModelProvider;
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
        var prompt = string.Format(
            _resourcesService.GetString("ask_prompt", context.Culture),
            context.Target,
            context.Sender.Name,
            _configuration.Name,
            room.Name,
            string.Join(", ", room.LastMessages.Select(pair => $"{pair.Item1}: {pair.Item2}")));

        var response = await _languageModelProvider.AskLanguageModelAsync(prompt, cancellationToken);
        if (response == null)
        {
            context.ReplyLocalizedMessage("ask_error");
            return;
        }
        context.Reply(response);
    }
}