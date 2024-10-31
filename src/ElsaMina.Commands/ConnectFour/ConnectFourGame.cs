using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.ConnectFour;

public class ConnectFourGame : Game
{
    private const char DEFAULT_CHARACTER = '_';

    public static int GameId { get; private set; }

    private readonly IRandomService _randomService;
    private readonly ITemplatesManager _templatesManager;
    private readonly IConfigurationManager _configurationManager;

    private bool _isStarted;
    private CancellationTokenSource _cancellationTokenSource;

    public ConnectFourGame(IRandomService randomService,
        ITemplatesManager templatesManager,
        IConfigurationManager configurationManager)
    {
        _randomService = randomService;
        _templatesManager = templatesManager;
        _configurationManager = configurationManager;

        GameId++;
    }

    #region Properties

    public List<IUser> Players { get; } = [];

    public List<(int, int)> WinningLineIndices { get; private set; } = [];

    public IUser PlayerCurrentlyPlaying { get; private set; }

    public char CurrentPlayerSymbol { get; private set; }

    public int TurnCount { get; private set; }

    public (int, int) LastPlayIndices { get; private set; } = (-1, -1);

    public char[,] Grid { get; } = new char[ConnectFourConstants.GRID_HEIGHT, ConnectFourConstants.GRID_WIDTH];

    public override string Identifier => nameof(ConnectFourGame);

    #endregion

    #region Public Methods

    public async Task JoinGame(IUser user)
    {
        if (_isStarted)
        {
            return;
        }

        if (Players.Contains(user))
        {
            return;
        }

        Players.Add(user);
        if (Players.Count >= ConnectFourConstants.MAX_PLAYERS_COUNT)
        {
            await StartGame();
        }
    }

    public async Task Play(IUser user, string playedColumn)
    {
        if (!_isStarted || !Equals(user, PlayerCurrentlyPlaying))
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
        while (i >= 0 && Grid[i, playedColumnIndex] != DEFAULT_CHARACTER)
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

        await InitializeNextTurn();
    }

    public async Task OnTimeout()
    {
        Players.Remove(PlayerCurrentlyPlaying);
        Context.Reply($"{PlayerCurrentlyPlaying.Name} were disqualified because they could not play in time.");

        if (Players.Count == 1)
        {
            await OnWin(Players[0]);
        }
        else
        {
            await InitializeNextTurn();
        }
    }

    public override void Cancel()
    {
        base.Cancel();
        _cancellationTokenSource?.Cancel();
        try
        {
            _cancellationTokenSource?.Dispose();
        }
        catch (ObjectDisposedException)
        {
            // Ignore
        }
    }

    #endregion

    #region Private Methods

    private async Task CheckWin(char symbol)
    {
        List<List<(int, int)>> checks =
        [
            CheckWinDirection(symbol, 0, 1), // Horizontal
            CheckWinDirection(symbol, 1, 0), // Vertical
            CheckWinDirection(symbol, 1, 1), // Diagonal (top-left to bottom-right)
            CheckWinDirection(symbol, 1, -1) // Diagonal (top-right to bottom-left)
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
        for (var i = 0; i < ConnectFourConstants.GRID_HEIGHT; i++)
        {
            for (var j = 0; j < ConnectFourConstants.GRID_WIDTH; j++)
            {
                if (Grid[i, j] == DEFAULT_CHARACTER)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private List<(int, int)> CheckWinDirection(char symbol, int rowOffset, int colOffset)
    {
        for (var i = 0; i < ConnectFourConstants.GRID_HEIGHT; i++)
        {
            for (var j = 0; j < ConnectFourConstants.GRID_WIDTH; j++)
            {
                var currentIndices = new List<(int, int)>();
                for (var k = 0; k < ConnectFourConstants.WINNING_LENGTH; k++)
                {
                    var row = i + k * rowOffset;
                    var col = j + k * colOffset;

                    if (row < 0
                        || row >= ConnectFourConstants.GRID_HEIGHT
                        || col < 0
                        || col >= ConnectFourConstants.GRID_WIDTH)
                    {
                        currentIndices.Clear();
                        break;
                    }

                    currentIndices.Add((row, col));

                    if (Grid[row, col] != symbol)
                    {
                        currentIndices.Clear();
                        break;
                    }
                }

                if (currentIndices.Count == ConnectFourConstants.WINNING_LENGTH)
                {
                    return currentIndices;
                }
            }
        }

        return null;
    }

    private async Task StartGame()
    {
        for (var i = 0; i < ConnectFourConstants.GRID_HEIGHT; i++)
        {
            for (var j = 0; j < ConnectFourConstants.GRID_WIDTH; j++)
            {
                Grid[i, j] = DEFAULT_CHARACTER;
            }
        }

        _isStarted = true;
        _randomService.ShuffleInPlace(Players);
        await InitializeNextTurn();
    }

    private async Task InitializeNextTurn()
    {
        TurnCount++;
        PlayerCurrentlyPlaying = Players[(TurnCount - 1) % ConnectFourConstants.MAX_PLAYERS_COUNT];
        CurrentPlayerSymbol = ConnectFourConstants.SYMBOLS[Players.IndexOf(PlayerCurrentlyPlaying)];
        await DisplayGrid();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(ConnectFourConstants.TIMEOUT_DELAY, token);
                token.ThrowIfCancellationRequested();
                await OnTimeout();
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
        }, token);
    }

    private async Task OnWin(IUser winner)
    {
        await DisplayGrid();
        if (winner is null)
        {
            Context.ReplyLocalizedMessage("c4_game_tie_end");
        }
        else
        {
            Context.ReplyLocalizedMessage("c4_game_win_message", winner.Name);
        }

        Cancel();
    }

    private async Task DisplayGrid()
    {
        var template = await _templatesManager.GetTemplate("ConnectFour/ConnectFourGameTable",
            new ConnectFourGameTableModel
            {
                Culture = Context.Culture,
                CurrentGame = this,
                BotName = _configurationManager.Configuration.Name,
                Trigger = _configurationManager.Configuration.Trigger
            });

        Context.SendHtmlPage($"c4-{GameId}", template.RemoveNewlines());
    }

    #endregion
}