using System.Timers;
using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using Timer = System.Timers.Timer;

namespace ElsaMina.Commands.ConnectFour;

public class ConnectFourGame : Game, IConnectFourGame
{
    private const char DEFAULT_CHARACTER = '_';

    private static int NextGameId { get; set; } = 1;

    private readonly IRandomService _randomService;
    private readonly ITemplatesManager _templatesManager;
    private readonly IConfiguration _configuration;
    private readonly IBot _bot;
    private readonly int _gameId;

    private Timer _timer;

    public ConnectFourGame(IRandomService randomService,
        ITemplatesManager templatesManager,
        IConfiguration configuration,
        IBot bot)
    {
        _randomService = randomService;
        _templatesManager = templatesManager;
        _configuration = configuration;
        _bot = bot;

        _gameId = NextGameId++;
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

    public string PlayerNames => string.Join(", ", Players.Select(player => player.Name));
    public int GameId => _gameId;

    public IContext Context { get; set; }
    
    private string AnnounceId => $"c4-announce-{_gameId}";

    #endregion

    #region Public Methods

    public async Task DisplayAnnounce()
    {
        var template = await _templatesManager.GetTemplateAsync("ConnectFour/ConnectFourGamePanel",
            new ConnectFourGamePanelViewModel
            {
                Culture = Context.Culture,
                BotName = _configuration.Name,
                ConnectFourGame = this,
                RoomId = Context.RoomId,
                Trigger = _configuration.Trigger
            });

        Context.SendUpdatableHtml(AnnounceId, template.RemoveNewlines(), true);
    }

    public async Task JoinGame(IUser user)
    {
        if (IsStarted)
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
        if (!IsStarted || !Equals(user, PlayerCurrentlyPlaying))
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

    public async Task Forfeit(IUser user)
    {
        if (!IsStarted || !Players.Contains(user))
        {
            return;
        }

        Context.ReplyLocalizedMessage("c4_game_player_forfeited", user.Name);
        await RemovePlayerAndCheckWin(user);
    }

    public async Task OnTimeout()
    {
        if (IsEnded)
        {
            return;
        }

        Context.ReplyLocalizedMessage("c4_game_on_timeout", PlayerCurrentlyPlaying.Name);
        await RemovePlayerAndCheckWin(PlayerCurrentlyPlaying);
    }

    private async Task RemovePlayerAndCheckWin(IUser player)
    {
        Players.Remove(player);

        if (Players.Count == 1)
        {
            await OnWin(Players[0]);
        }
        else
        {
            await InitializeNextTurn();
        }
    }

    public void Cancel()
    {
        OnEnd();
        CancelTimer();
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
        var passingChecks = checks.Where(check => check.Count > 0).ToArray();

        if (passingChecks.Length >= 1)
        {
            WinningLineIndices = passingChecks[0];
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

                    if (Grid[row, col] == symbol)
                    {
                        continue;
                    }

                    currentIndices.Clear();
                    break;
                }

                if (currentIndices.Count == ConnectFourConstants.WINNING_LENGTH)
                {
                    return currentIndices;
                }
            }
        }

        return [];
    }

    private async Task StartGame()
    {
        var ongoingGameMessage = Context.GetString("c4_panel_ongoing_game", PlayerNames);
        Context.SendUpdatableHtml(AnnounceId, ongoingGameMessage, true);

        for (var i = 0; i < ConnectFourConstants.GRID_HEIGHT; i++)
        {
            for (var j = 0; j < ConnectFourConstants.GRID_WIDTH; j++)
            {
                Grid[i, j] = DEFAULT_CHARACTER;
            }
        }

        OnStart();
        _randomService.ShuffleInPlace(Players);
        await InitializeNextTurn();
    }

    private async Task InitializeNextTurn()
    {
        TurnCount++;
        PlayerCurrentlyPlaying = Players[(TurnCount - 1) % ConnectFourConstants.MAX_PLAYERS_COUNT];
        CurrentPlayerSymbol = ConnectFourConstants.SYMBOLS[Players.IndexOf(PlayerCurrentlyPlaying)];
        await DisplayGrid();

        CancelTimer();

        _timer = new Timer(ConnectFourConstants.TIMEOUT_DELAY);
        _timer.AutoReset = false;
        _timer.Elapsed += HandleTimerElapsed;
        _timer.Start();
    }

    private void CancelTimer()
    {
        if (_timer == null)
        {
            return;
        } 
        _timer.Elapsed -= HandleTimerElapsed;
        _timer.Dispose();
    }

    private async void HandleTimerElapsed(object sender, ElapsedEventArgs e)
    {
        await OnTimeout();
        CancelTimer();
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
        var template = await _templatesManager.GetTemplateAsync("ConnectFour/ConnectFourGameTable",
            new ConnectFourGameTableModel
            {
                Culture = Context.Culture,
                RoomId = Context.RoomId,
                CurrentGame = this,
                BotName = _configuration.Name,
                Trigger = _configuration.Trigger
            });

        var pageName = $"c4-game-{Context.RoomId}-{_gameId}";
        var sanitizedTemplate = template.RemoveNewlines();
        foreach (var player in Players)
        {
            _bot.Say(Context.RoomId, $"/sendhtmlpage {player.Name}, {pageName}, {sanitizedTemplate}");
        }
    }

    #endregion
}