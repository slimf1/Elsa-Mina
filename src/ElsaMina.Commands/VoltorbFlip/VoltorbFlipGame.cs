using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.VoltorbFlip;

public class VoltorbFlipGame : Game, IVoltorbFlipGame
{
    private static int NextGameId { get; set; } = 1;
    
    private readonly IRandomService _randomService;
    private readonly ITemplatesManager _templatesManager;
    private readonly IConfiguration _configuration;

    private int _revealedCount;
    private int _revealedTwos;
    private int _revealedThrees;
    private int _targetTwos;
    private int _targetThrees;
    private readonly int _gameId;
    private readonly PeriodicTimerRunner _inactivityTimer;

    public VoltorbFlipGame(IRandomService randomService,
        ITemplatesManager templatesManager,
        IConfiguration configuration)
    {
        _randomService = randomService;
        _templatesManager = templatesManager;
        _configuration = configuration;
        _inactivityTimer = new PeriodicTimerRunner(VoltorbFlipConstants.INACTIVITY_TIMEOUT, OnInactivityTimeout, runOnce: true);

        _gameId = NextGameId++;
    }

    public override string Identifier => nameof(VoltorbFlipGame);

    public int Level { get; private set; } = 1;
    public bool IsRoundActive { get; private set; }
    public int CurrentCoins { get; private set; }
    public int[,] TileValues { get; private set; }
    public bool[,] IsRevealed { get; private set; }
    public int[] RowSums { get; private set; }
    public int[] ColSums { get; private set; }
    public int[] RowVoltorbs { get; private set; }
    public int[] ColVoltorbs { get; private set; }
    public IContext Context { get; set; }
    public IUser Owner { get; set; }
    private string GameIdentifier => $"vf-{Context.RoomId}-{_gameId}";

    public async Task StartNewRound()
    {
        const int size = VoltorbFlipConstants.GRID_SIZE;
        TileValues = new int[size, size];
        IsRevealed = new bool[size, size];
        RowSums = new int[size];
        ColSums = new int[size];
        RowVoltorbs = new int[size];
        ColVoltorbs = new int[size];
        _revealedCount = 0;
        _revealedTwos = 0;
        _revealedThrees = 0;
        CurrentCoins = 0;

        var configs = VoltorbFlipConstants.LEVEL_CONFIGURATIONS[Level - 1];
        var config = _randomService.RandomElement(configs);
        _targetTwos = config.Twos;
        _targetThrees = config.Threes;

        var tiles = new List<int>(size * size);
        for (var i = 0; i < config.Voltorbs; i++)
        {
            tiles.Add(0);
        }

        for (var i = 0; i < config.Threes; i++)
        {
            tiles.Add(3);
        }

        for (var i = 0; i < config.Twos; i++)
        {
            tiles.Add(2);
        }

        while (tiles.Count < size * size)
        {
            tiles.Add(1);
        }

        _randomService.ShuffleInPlace(tiles);

        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                TileValues[row, col] = tiles[row * size + col];
            }
        }

        ComputeRowColStats();
        IsRoundActive = true;

        if (!IsStarted)
        {
            OnStart();
        }

        _inactivityTimer.Restart();
        Context.ReplyLocalizedMessage("vf_game_started", Level);
        await DisplayBoard(showAll: false, firstTime: true);
    }

    public async Task FlipTile(IUser user, int row, int col)
    {
        if (!IsRoundActive || user.UserId != Owner.UserId)
        {
            return;
        }

        const int size = VoltorbFlipConstants.GRID_SIZE;
        if (row < 0 || row >= size || col < 0 || col >= size)
        {
            return;
        }

        if (IsRevealed[row, col])
        {
            return;
        }

        IsRevealed[row, col] = true;
        var value = TileValues[row, col];

        if (value == 0)
        {
            IsRoundActive = false;
            var newLevel = ComputeNewLevelOnLossOrQuit();
            Level = newLevel;
            Context.ReplyLocalizedMessage("vf_game_voltorb_hit", user.Name, newLevel);
            await DisplayBoard(showAll: true, firstTime: false);
            return;
        }

        _revealedCount++;
        if (value == 2)
        {
            _revealedTwos++;
        }
        else if (value == 3)
        {
            _revealedThrees++;
        }

        UpdateCoins();

        if (_revealedTwos >= _targetTwos && _revealedThrees >= _targetThrees)
        {
            IsRoundActive = false;
            var earnedCoins = CurrentCoins;
            var newLevel = Math.Min(Level + 1, VoltorbFlipConstants.MAX_LEVEL);
            Level = newLevel;
            Context.ReplyLocalizedMessage("vf_game_win", user.Name, earnedCoins, newLevel);
            await DisplayBoard(showAll: true, firstTime: true);
            return;
        }

        _inactivityTimer.Restart();
        await DisplayBoard(showAll: false, firstTime: true);
    }

    public async Task QuitRound(IUser user)
    {
        if (!IsRoundActive || user.UserId != Owner.UserId)
        {
            return;
        }

        IsRoundActive = false;
        var earnedCoins = CurrentCoins;
        var newLevel = ComputeNewLevelOnLossOrQuit();
        Level = newLevel;
        Context.ReplyLocalizedMessage("vf_game_quit", user.Name, earnedCoins, newLevel);
        await DisplayBoard(showAll: true, firstTime: true);
    }

    public void Cancel()
    {
        IsRoundActive = false;
        _inactivityTimer.Stop();
        OnEnd();
    }

    private async Task OnInactivityTimeout()
    {
        if (IsEnded)
        {
            return;
        }

        Context.ReplyLocalizedMessage("vf_game_timeout");
        Cancel();
        await DisplayBoard(showAll: true, firstTime: false);
    }

    private void ComputeRowColStats()
    {
        const int size = VoltorbFlipConstants.GRID_SIZE;
        for (var row = 0; row < size; row++)
        {
            RowSums[row] = 0;
            RowVoltorbs[row] = 0;
            for (var col = 0; col < size; col++)
            {
                var value = TileValues[row, col];
                if (value == 0)
                {
                    RowVoltorbs[row]++;
                }
                else
                {
                    RowSums[row] += value;
                }
            }
        }

        for (var col = 0; col < size; col++)
        {
            ColSums[col] = 0;
            ColVoltorbs[col] = 0;
            for (var row = 0; row < size; row++)
            {
                var value = TileValues[row, col];
                if (value == 0)
                {
                    ColVoltorbs[col]++;
                }
                else
                {
                    ColSums[col] += value;
                }
            }
        }
    }

    private void UpdateCoins()
    {
        CurrentCoins = _revealedCount > 0
            ? (int)(Math.Pow(2, _revealedTwos) * Math.Pow(3, _revealedThrees))
            : 0;
    }

    private int ComputeNewLevelOnLossOrQuit()
    {
        return _revealedCount < Level
            ? Math.Max(1, _revealedCount)
            : Level;
    }

    private async Task DisplayBoard(bool showAll, bool firstTime)
    {
        var template = await _templatesManager.GetTemplateAsync("VoltorbFlip/VoltorbFlipBoard",
            new VoltorbFlipModel
            {
                Culture = Context.Culture,
                CurrentGame = this,
                BotName = _configuration.Name,
                Trigger = _configuration.Trigger,
                RoomId = Context.RoomId,
                ShowAll = showAll
            });

        Context.SendUpdatableHtml(GameIdentifier, template.RemoveNewlines(), !firstTime);
    }
}