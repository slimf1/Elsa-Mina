using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
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
    public override Rank RequiredRank => Rank.Regular;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var language = context.Command == "factde" ? "de" : "en";
        try
        {
            var response = await _httpService.GetAsync<FactDto>(string.Format(FACTS_URL, language));
            context.Reply($"**Fact**: {response.Data.Text}", rankAware: true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Could not fetch fact.");
            context.ReplyLocalizedMessage("fact_error");
        }
    }
}