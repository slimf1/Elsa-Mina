using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.GuessingGame;

public abstract class GuessingGame : Game, IGuessingGame
{
    private const int DEFAULT_TURNS_COUNT = 10;
    private const int MIN_LENGTH_FOR_AUTOCORRECT = 8;
    private static readonly TimeSpan TIME_WARNING_THRESHOLD = TimeSpan.FromSeconds(5);
    protected static readonly TimeSpan DEFAULT_TURN_COOLDOWN = TimeSpan.FromSeconds(15);

    private readonly ITemplatesManager _templatesManager;
    private readonly IConfiguration _configuration;
    private PeriodicTimerRunner _timer;
    private int _elapsedSeconds;

    private static readonly TimeSpan ANSWER_RATE_LIMIT = TimeSpan.FromSeconds(2);

    private readonly Dictionary<string, int> _scores = new();
    private readonly Dictionary<string, DateTimeOffset> _lastAnswerTimes = new();
    private readonly IClockService _clockService;
    private bool _hasRoundBeenWon;

    protected GuessingGame(ITemplatesManager templatesManager,
        IConfiguration configuration, IClockService clockService)
    {
        _templatesManager = templatesManager;
        _configuration = configuration;
        _clockService = clockService;
    }

    protected IReadOnlyDictionary<string, int> Scores => _scores;

    protected bool HasRoundBeenWon => _hasRoundBeenWon;

    protected int CurrentTurn { get; private set; }

    protected IEnumerable<string> CurrentValidAnswers { get; set; } = [];

    public int TurnsCount { get; set; } = DEFAULT_TURNS_COUNT;

    public IContext Context { get; set; }

    public async Task Start()
    {
        OnStart();
        OnGameStart();
        await InitializeNextTurn();
    }

    private async Task InitializeNextTurn()
    {
        CurrentTurn++;
        Context.ReplyLocalizedMessage("guessing_game_turn_count", CurrentTurn);
        await OnTurnStart();
        _hasRoundBeenWon = false;

        CancelTimer();
        _elapsedSeconds = 0;
        _timer = new PeriodicTimerRunner(TimeSpan.FromSeconds(1), HandleTimerTickAsync);
        _timer.Start();
    }

    private void CancelTimer()
    {
        _timer?.Dispose();
        _timer = null;
    }

    private async Task HandleTimerTickAsync()
    {
        _elapsedSeconds++;
        var remainingTime = DEFAULT_TURN_COOLDOWN - TimeSpan.FromSeconds(_elapsedSeconds);
        OnTimerCountdown(remainingTime);
        if (remainingTime > TimeSpan.Zero)
        {
            return;
        }

        CancelTimer();

        await OnTurnEnd();
    }

    private async Task OnTurnEnd()
    {
        if (!_hasRoundBeenWon)
        {
            Context.ReplyLocalizedMessage("guessing_game_answer_not_found",
                string.Join(", ", CurrentValidAnswers.Distinct()));
        }

        if (CurrentTurn >= TurnsCount || IsEnded)
        {
            await EndGame();
        }
        else
        {
            await InitializeNextTurn();
        }
    }

    public void OnAnswer(string userName, string answer)
    {
        if (_hasRoundBeenWon ||
            userName.ToLowerAlphaNum() == _configuration.Name.ToLowerAlphaNum())
        {
            return;
        }

        var userId = userName.ToLowerAlphaNum();
        var now = _clockService.CurrentUtcDateTime;
        if (_lastAnswerTimes.TryGetValue(userId, out var lastAnswerTime) &&
            now - lastAnswerTime < ANSWER_RATE_LIMIT)
        {
            return;
        }

        _lastAnswerTimes[userId] = now;
        var maxLevenshteinDistance = answer.Length > MIN_LENGTH_FOR_AUTOCORRECT ? 1 : 0;
        if (!CurrentValidAnswers.Any(validAnswer =>
                validAnswer.ToLowerAlphaNum().LevenshteinDistance(answer.ToLowerAlphaNum()) <=
                maxLevenshteinDistance))
        {
            // Aucune réponse n'est suffisamment proche
            return;
        }

        _hasRoundBeenWon = true;
        _scores.TryAdd(userId, 0);

        _scores[userId] += 1;
        Context.ReplyLocalizedMessage("guessing_game_round_won",
            userName,
            _scores[userId],
            _scores[userId] == 1 ? string.Empty : "s");
        OnCorrectAnswer();
    }

    private async Task EndGame()
    {
        StopGame();

        var resultViewModel = new GuessingGameResultViewModel
        {
            Culture = Context.Culture,
            Scores = _scores
        };
        var template = await _templatesManager.GetTemplateAsync("GuessingGame/GuessingGameResult", resultViewModel);
        Context.ReplyHtml(template.RemoveNewlines());
    }

    public void StopGame()
    {
        OnEnd();
        CancelTimer();
    }

    protected virtual void OnTimerCountdown(TimeSpan remainingTime)
    {
        if (remainingTime == TIME_WARNING_THRESHOLD && !_hasRoundBeenWon)
        {
            Context.ReplyLocalizedMessage("countries_game_turn_ending_soon", remainingTime.Seconds);
        }
    }

    protected virtual void OnCorrectAnswer()
    {
    }

    protected abstract void OnGameStart();

    protected abstract Task OnTurnStart();
}