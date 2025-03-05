using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Misc.Bitcoin;

[NamedCommand("bitcoin", Aliases = ["btc"])]
public class BitcoinCommand : Command
{
    private const string COINDESK_API_URL = "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=usd,eur";

    private readonly IHttpService _httpService;

    public BitcoinCommand(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override Rank RequiredRank => Rank.Regular;

    public override async Task Run(IContext context)
    {
        try
        {
            var result = await _httpService.Get<IDictionary<string, IDictionary<string, int>>>(COINDESK_API_URL);
            var coinValues = result.Data["bitcoin"];
            var eur = coinValues["eur"];
            var usd = coinValues["usd"];
            context.Reply($"1 bitcoin = {eur}€ = {usd}$", rankAware: true);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Could not fetch data from coindesk.");
            context.ReplyLocalizedMessage("bitcoin_error");
        }
    }
}