using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.ConnectFour;

public class ConnectFour : Game
{
    public static int GameId { get; private set; }

    private readonly IRandomService _randomService;
    private readonly ITemplatesManager _templatesManager;
    private readonly IConfigurationManager _configurationManager;

    private bool _isStarted;
    private bool _hasEnded;
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

    public List<(int, int)> WinningLineIndices { get; private set; } = [];

    public string PlayerCurrentlyPlaying { get; private set; }

    public char CurrentPlayerSymbol => ConnectFourConstants.SYMBOLS[Players.IndexOf(PlayerCurrentlyPlaying)];

    public int TurnCount { get; private set; }

    public (int, int) LastPlayIndices { get; private set; }

    public char[,] Grid { get; } = new char[ConnectFourConstants.GRID_HEIGHT, ConnectFourConstants.GRID_WIDTH];

    public override string Identifier => nameof(ConnectFour);

    public async Task JoinGame(string userName)
    {
        if (_isStarted)
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

    public async Task Play(string user, string playedColumn)
    {
        if (!_isStarted || user.ToLowerAlphaNum() != PlayerCurrentlyPlaying)
        {
            return;
        }

        if (!int.TryParse(playedColumn, out var playedColumnIndex))
        {
            return;
        }

        if (playedColumnIndex is < 1 or > ConnectFourConstants.GRID_WIDTH)
        {
            return;
        }

        playedColumnIndex -= 1;
        var i = ConnectFourConstants.GRID_HEIGHT - 1;
        while (Grid[i, playedColumnIndex] != default)
        {
            i--;
        }

        if (i < 0)
        {
            return; // La colonne est remplie
        }

        Grid[i, playedColumnIndex] = CurrentPlayerSymbol;
        LastPlayIndices = (i, playedColumnIndex);

        foreach (var symbol in ConnectFourConstants.SYMBOLS)
        {
            await CheckWin(symbol);
        }
        InitializeNextTurn();
    }

    private async Task CheckWin(char symbol)
    {
        List<List<(int, int)>> checks = [
            CheckLines(symbol),
            CheckColumns(symbol),
            CheckDiagonals(symbol)
        ];
        var passingChecks = checks.Where(check => check != null).ToArray();

        if (passingChecks.Length >= 1)
        {
            WinningLineIndices = passingChecks.First();
            var winner = Players[Array.IndexOf(ConnectFourConstants.SYMBOLS, symbol)];
            await OnWin(winner);
        }
        else if (CheckTie())
        {
            await OnWin(null);
        }
    }

    private bool CheckTie()
    {
        return Grid.Cast<char>().All(cell => cell != default);
    }

    private List<(int, int)> CheckDiagonals(char symbol)
    {
        for (var i = 0; i < ConnectFourConstants.GRID_HEIGHT; i++)
        {
            for (var j = 0; j < ConnectFourConstants.GRID_WIDTH; j++)
            {
                
            }
        }

        return null;
    }

    private List<(int, int)> CheckColumns(char symbol)
    {
        return null;
    }

    private List<(int, int)> CheckLines(char symbol)
    {
        return null;
    }

    private async Task StartGame()
    {
        _isStarted = true;
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
            await Task.Delay(ConnectFourConstants.TIMEOUT_DELAY, _cancellationTokenSource.Token);
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
            await OnTimeout();
        }, _cancellationTokenSource.Token);
    }

    private async Task OnTimeout()
    {
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
        if (string.IsNullOrWhiteSpace(winner))
        {
            Context.ReplyLocalizedMessage("c4_game_tie_end");
        }
        else
        {
            Context.ReplyLocalizedMessage("c4_game_win_message", winner);
        }

        OnEnd();
    }

    private void OnEnd()
    {
        _hasEnded = true;
        _cancellationTokenSource?.Cancel();
    }

    public override void Cancel()
    {
        base.Cancel();
        _hasEnded = true;
        _cancellationTokenSource?.Cancel();
    }

    private async Task DisplayGrid()
    {
        var template = await _templatesManager.GetTemplate("ConnectFour/ConnectFourGameTable", new ConnectFourGridModel
        {
            Culture = Context.Culture,
            CurrentGame = this,
            BotName = _configurationManager.Configuration.Name,
            Trigger = _configurationManager.Configuration.Trigger
        });

        Context.SendHtmlPage($"c4-{GameId}", template.RemoveNewlines());
    }
}