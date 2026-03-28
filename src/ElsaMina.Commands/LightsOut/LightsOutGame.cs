using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.LightsOut;

public class LightsOutGame : Game, ILightsOutGame
{
    private static int NextGameId { get; set; } = 1;

    private readonly IRandomService _randomService;
    private readonly ITemplatesManager _templatesManager;
    private readonly IConfiguration _configuration;
    private readonly IBotDbContextFactory _dbContextFactory;

    private readonly int _gameId;
    private readonly PeriodicTimerRunner _inactivityTimer;

    public LightsOutGame(IRandomService randomService,
        ITemplatesManager templatesManager,
        IConfiguration configuration,
        IBotDbContextFactory dbContextFactory)
    {
        _randomService = randomService;
        _templatesManager = templatesManager;
        _configuration = configuration;
        _dbContextFactory = dbContextFactory;
        _inactivityTimer = new PeriodicTimerRunner(LightsOutConstants.INACTIVITY_TIMEOUT, OnInactivityTimeout, runOnce: true);

        _gameId = NextGameId++;
    }

    public override string Identifier => nameof(LightsOutGame);

    public int Level { get; private set; } = 1;
    public int MoveCount { get; private set; }
    public int BestScore { get; private set; }
    public bool IsRoundActive { get; private set; }
    public bool[,] Grid { get; private set; }
    public bool IsPrivateMode { get; set; }
    public string TargetRoomId { get; set; }
    public string TargetUserId { get; set; }
    public IContext Context { get; set; }
    public IUser Owner { get; set; }

    private string EffectiveRoomId => IsPrivateMode ? TargetRoomId : Context.RoomId;
    private string GameIdentifier => $"lo-{EffectiveRoomId}-{_gameId}";
    private string AnnounceId => GameIdentifier;

    public async Task DisplayAnnounce()
    {
        var template = await _templatesManager.GetTemplateAsync("LightsOut/LightsOutAnnounce",
            new LightsOutModel
            {
                Culture = Context.Culture,
                CurrentGame = this,
                BotName = _configuration.Name,
                Trigger = _configuration.Trigger,
                RoomId = EffectiveRoomId,
                IsPrivateMode = IsPrivateMode
            });

        _inactivityTimer.Restart();
        Context.SendUpdatableHtml(AnnounceId, template.RemoveNewlines(), false);
    }

    public async Task StartNewRound()
    {
        var savedData = await LoadPlayerDataAsync();
        Level = savedData?.Level ?? 1;
        BestScore = savedData?.BestMoves ?? 0;

        const int size = LightsOutConstants.GRID_SIZE;
        Grid = new bool[size, size];
        MoveCount = 0;

        GenerateSolvablePuzzle();

        IsRoundActive = true;
        if (!IsStarted)
        {
            OnStart();
        }

        _inactivityTimer.Restart();
        Context.ReplyLocalizedMessage("lo_game_started", Level);
        await DisplayBoard(firstTime: true);
    }

    public async Task ToggleCell(IUser user, int row, int col)
    {
        if (!IsRoundActive || user.UserId != Owner.UserId)
        {
            return;
        }

        const int size = LightsOutConstants.GRID_SIZE;
        if (row < 0 || row >= size || col < 0 || col >= size)
        {
            return;
        }

        Toggle(row, col);
        if (row > 0) Toggle(row - 1, col);
        if (row < size - 1) Toggle(row + 1, col);
        if (col > 0) Toggle(row, col - 1);
        if (col < size - 1) Toggle(row, col + 1);

        MoveCount++;
        _inactivityTimer.Restart();

        if (IsSolved())
        {
            IsRoundActive = false;
            Level = Math.Min(Level + 1, LightsOutConstants.MAX_LEVEL);
            Context.ReplyLocalizedMessage("lo_game_win", Owner.Name, MoveCount, Level);
            await SavePlayerDataAsync();
            _inactivityTimer.Stop();
            OnEnd();
        }

        await DisplayBoard(firstTime: false);
    }

    public async Task CancelAsync()
    {
        IsRoundActive = false;
        _inactivityTimer.Stop();
        OnEnd();
        await DisplayBoard(firstTime: false);
    }

    private void Toggle(int row, int col)
    {
        Grid[row, col] = !Grid[row, col];
    }

    private bool IsSolved()
    {
        const int size = LightsOutConstants.GRID_SIZE;
        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                if (Grid[row, col])
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void GenerateSolvablePuzzle()
    {
        const int size = LightsOutConstants.GRID_SIZE;

        // Generate a solvable puzzle by applying random toggles from a solved state
        // Number of random presses scales with level
        var pressCount = Level + 2;

        for (var press = 0; press < pressCount; press++)
        {
            var row = _randomService.NextInt(size);
            var col = _randomService.NextInt(size);

            Toggle(row, col);
            if (row > 0) Toggle(row - 1, col);
            if (row < size - 1) Toggle(row + 1, col);
            if (col > 0) Toggle(row, col - 1);
            if (col < size - 1) Toggle(row, col + 1);
        }

        // Make sure the puzzle isn't already solved
        if (IsSolved())
        {
            var row = _randomService.NextInt(size);
            var col = _randomService.NextInt(size);
            Toggle(row, col);
            if (row > 0) Toggle(row - 1, col);
            if (row < size - 1) Toggle(row + 1, col);
            if (col > 0) Toggle(row, col - 1);
            if (col < size - 1) Toggle(row, col + 1);
        }
    }

    private async Task OnInactivityTimeout()
    {
        if (IsEnded)
        {
            return;
        }

        Context.ReplyLocalizedMessage("lo_game_timeout");
        await CancelAsync();
    }

    private async Task<LightsOutScore> LoadPlayerDataAsync()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.LightsOutScores.FindAsync(Owner.UserId);
    }

    private async Task SavePlayerDataAsync()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        var record = await db.LightsOutScores.FindAsync(Owner.UserId);
        if (record == null)
        {
            await db.LightsOutScores.AddAsync(new LightsOutScore
            {
                UserId = Owner.UserId,
                Level = Level,
                BestMoves = MoveCount
            });
        }
        else
        {
            record.Level = Math.Max(record.Level, Level);
            if (record.BestMoves == 0 || MoveCount < record.BestMoves)
            {
                record.BestMoves = MoveCount;
            }
        }
        await db.SaveChangesAsync();
    }

    private async Task DisplayBoard(bool firstTime)
    {
        var template = await _templatesManager.GetTemplateAsync("LightsOut/LightsOutBoard",
            new LightsOutModel
            {
                Culture = Context.Culture,
                CurrentGame = this,
                BotName = _configuration.Name,
                Trigger = _configuration.Trigger,
                RoomId = EffectiveRoomId,
                IsPrivateMode = IsPrivateMode
            });

        if (IsPrivateMode)
        {
            Context.SendPrivateUpdatableHtml(TargetUserId, TargetRoomId, GameIdentifier, template.RemoveNewlines(), !firstTime);
        }
        else
        {
            Context.SendUpdatableHtml(GameIdentifier, template.RemoveNewlines(), !firstTime);
        }
    }
}
