using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Http;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Misc.Pairings;

[NamedCommand("pairings", "makepairings")]
public class PairingsCommand : Command
{
    private readonly IHttpService _httpService;
    private readonly IRandomService _randomService;

    public PairingsCommand(IHttpService httpService, IRandomService randomService)
    {
        _httpService = httpService;
        _randomService = randomService;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override Rank RequiredRank => Rank.Regular;
    public override string HelpMessageKey => "pairings_help";

    private static int GetByes(int playerCount)
    {
        if (playerCount == 0)
            return 0;
            
        var nextPowerOfTwo = 1;
        while (nextPowerOfTwo < playerCount)
        {
            nextPowerOfTwo *= 2;
        }
        return nextPowerOfTwo - playerCount;
    }

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        List<string> playerNames;
        
        if (context.Target.Contains("hastebin.com/") || context.Target.Contains("pastebin.com/"))
        {
            var urlParts = context.Target.Split('/');
            var pasteId = urlParts[^1];
            var host = urlParts[^2];
            var response = await _httpService.GetAsync<string>($"https://{host}/raw/{pasteId}", 
                isRaw: true, cancellationToken: cancellationToken);
            playerNames = response.Data.Split('\n').ToList();
        }
        else
        {
            playerNames = context.Target.Split(',').Select(name => name.Trim()).ToList();
        }

        playerNames = playerNames.Where(name => !string.IsNullOrWhiteSpace(name)).ToList();
        
        if (playerNames.Count == 0)
        {
            context.SendHtmlIn("", rankAware: true);
            return;
        }
        
        var availablePlayers = new List<string>(playerNames);
        _randomService.ShuffleInPlace(availablePlayers);
        
        var numberOfByes = GetByes(availablePlayers.Count);
        var matches = new List<string>();

        for (var index = 0; index < numberOfByes; index++)
        {
            var player = availablePlayers[^1];
            availablePlayers.RemoveAt(availablePlayers.Count - 1);
            matches.Add($"{player} vs. Bye #{index + 1}");
        }

        while (availablePlayers.Count >= 2)
        {
            var player1 = availablePlayers[^1];
            var player2 = availablePlayers[^2];
            availablePlayers.RemoveRange(availablePlayers.Count - 2, 2);
            matches.Add($"{player1} vs. {player2}");
        }

        context.SendHtmlIn(string.Join("<br />", matches), rankAware: true);
    }
} 