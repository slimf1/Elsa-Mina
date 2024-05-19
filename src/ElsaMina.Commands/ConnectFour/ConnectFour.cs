using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.ConnectFour;

public class ConnectFour : Game
{
    public static int GameId { get; private set; }

    private static readonly TimeSpan TIMEOUT_DELAY = TimeSpan.FromSeconds(30);

    private readonly IRandomService _randomService;
    private readonly ITemplatesManager _templatesManager;
    private readonly IConfigurationManager _configurationManager;

    private bool _started = false;
    private bool _ended = false;
    private CancellationTokenSource _cancellationTokenSource;

    public ConnectFour(IRandomService randomService,
        ITemplatesManager templatesManager,
        IConfigurationManager configurationManager)
    {
        _randomService = randomService;
        _templatesManager = templatesManager;
        _configurationManager = configurationManager;

        GameId++;
    }

    public List<string> Players { get; } = [];
    
    public List<(int, int)> WinningLineIndices { get; } = [];
    
    public string PlayerCurrentlyPlaying { get; private set; }
    
    public int TurnCount { get; private set; }
    
    public (int, int) LastPlayIndices { get; private set; }

    public char[,] Grid { get; } = new char[ConnectFourConstants.GRID_HEIGHT, ConnectFourConstants.GRID_WIDTH];
    
    public override string Identifier => nameof(ConnectFour);

    public async Task JoinGame(string userName)
    {
        if (_started)
        {
            return;
        }

        var userId = userName.ToLowerAlphaNum();
        if (Players.Contains(userId))
        {
            return;
        }

        Players.Add(userName);
        if (Players.Count >= ConnectFourConstants.MAX_PLAYERS_COUNT)
        {
            await StartGame();
        }
    }

    private async Task StartGame()
    {
        _started = true;
        _randomService.ShuffleInPlace(Players);
        await DisplayGrid();
        InitializeNextTurn();
    }

    private void InitializeNextTurn()
    {
        TurnCount++;
        PlayerCurrentlyPlaying = Players[TurnCount % ConnectFourConstants.MAX_PLAYERS_COUNT];
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(async () =>
        {
            await Task.Delay(TIMEOUT_DELAY, _cancellationTokenSource.Token);
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
            await OnTimeout();
        }, _cancellationTokenSource.Token);
    }

    private async Task OnTimeout()
    {
        // Disqualify the current player
        Players.Remove(PlayerCurrentlyPlaying);
        Context.Reply($"{PlayerCurrentlyPlaying} was disqualified because they could not play in time.");

        if (Players.Count == 1)
        {
            await OnWin(Players[0]);
        }
        else
        {
            InitializeNextTurn();
        }
    }

    private async Task OnWin(string winner)
    {
        await DisplayGrid();
        Context.Reply(
            string.IsNullOrEmpty(winner)
                ? "The game ended in a tie."
                : $"{winner} won the game!"
        );
    }

    private async Task DisplayGrid()
    {
        await _templatesManager.GetTemplate("ConnectFour/ConnectFourGrid", new ConnectFourGridModel
        {
            Culture = Context.Culture,
            CurrentGame = this,
            BotName = _configurationManager.Configuration.Name,
            Trigger = _configurationManager.Configuration.Trigger
        });
    }
}