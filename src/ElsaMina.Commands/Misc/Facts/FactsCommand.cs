using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Misc.Facts;

[NamedCommand("fact", Aliases = ["facts", "factde"])]
public class FactsCommand : Command
{
    private const string FACTS_URL = "https://uselessfacts.jsph.pl/random.json?language={0}";

    private readonly IHttpService _httpService;

    public FactsCommand(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override char RequiredRank => '+';

    public override async Task Run(IContext context)
    {
        var language = context.Command == "factde" ? "de" : "en";
        try
        {
            var result = await _httpService.Get<FactDto>(string.Format(FACTS_URL, language));
            context.Reply($"**Fact**: {result.Text}");
        }
        catch (Exception ex)
        {
            Logger.Current.Error(ex, "Could not fetch fact.");
        }
    }
}