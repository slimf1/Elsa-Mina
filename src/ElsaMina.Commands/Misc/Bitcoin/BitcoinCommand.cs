using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Misc.Bitcoin;

[NamedCommand("bitcoin", Aliases = ["btc"])]
public class BitcoinCommand : Command
{
    private const string COINDESK_API_URL = "https://api.coindesk.com/v1/bpi/currentprice.json";

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
            var result = await _httpService.Get<CoinDeskResponseDto>(COINDESK_API_URL);
            var data = result.Data;
            var eur = data.Bpi["EUR"].Rate;
            var usd = data.Bpi["USD"].Rate;
            context.Reply($"1 bitcoin = {eur:F2}€ = {usd:F2}$", rankAware: true);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Could not fetch data from coindesk.");
            context.ReplyLocalizedMessage("bitcoin_error");
        }
    }
}