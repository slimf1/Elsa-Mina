using ElsaMina.Core;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
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
    public override char RequiredRank => '+';

    public override async Task Run(IContext context)
    {
        try
        {
            var result = await _httpService.Get<CoinDeskResponseDto>(COINDESK_API_URL);
            var eur = result.Bpi["EUR"].Rate;
            var usd = result.Bpi["USD"].Rate;
            context.Reply($"1 bitcoin = {eur:F2}€ = {usd:F2}$");
        }
        catch (Exception ex)
        {
            Logger.Current.Error(ex, "Could not fetch data from coindesk.");
        }
    }
}