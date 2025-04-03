using System.Timers;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using Timer = System.Timers.Timer;

namespace ElsaMina.Commands.GuessingGame;

public abstract class GuessingGame : Game, IGuessingGame
{
    private const int DEFAULT_TURNS_COUNT = 10;
    private const int MIN_LENGTH_FOR_AUTOCORRECT = 8;
    private static readonly TimeSpan TURN_COOLDOWN = TimeSpan.FromSeconds(15);

    private readonly ITemplatesManager _templatesManager;
    private readonly IConfigurationManager _configurationManager;
    private Timer _timer;

    private readonly Dictionary<string, int> _scores = new();
    private int _currentTurn;
    private bool _hasRoundBeenWon;

    protected GuessingGame(ITemplatesManager templatesManager,
        IConfigurationManager configurationManager)
    {
        _templatesManager = templatesManager;
        _configurationManager = configurationManager;
    }

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
        _currentTurn++;
        Context.ReplyLocalizedMessage("guessing_game_turn_count", _currentTurn);
        await OnTurnStart();
        _hasRoundBeenWon = false;

        CancelTimer();
        _timer = new Timer(TURN_COOLDOWN)
        {
            AutoReset = false
        };
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
        _timer = null;
    }

    private async void HandleTimerElapsed(object sender, ElapsedEventArgs e)
    {
        await OnTurnEnd();
    }

    private async Task OnTurnEnd()
    {
        if (!_hasRoundBeenWon)
        {
            Context.ReplyLocalizedMessage("guessing_game_answer_not_found",
                string.Join(", ", CurrentValidAnswers.Distinct()));
        }

        if (_currentTurn >= TurnsCount || IsEnded)
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
            userName.ToLowerAlphaNum() == _configurationManager.Configuration.Name.ToLowerAlphaNum())
        {
            return;
        }

        var userId = userName.ToLowerAlphaNum();
        var maxLevenshteinDistance = answer.Length > MIN_LENGTH_FOR_AUTOCORRECT ? 1 : 0;
        if (!CurrentValidAnswers.Any(validAnswer =>
                Text.LevenshteinDistance(validAnswer.ToLowerAlphaNum(), answer.ToLowerAlphaNum()) <=
                maxLevenshteinDistance))
        {
            return;
        }

        _hasRoundBeenWon = true;
        _scores.TryAdd(userId, 0);

        _scores[userId] += 1;
        Context.ReplyLocalizedMessage("guessing_game_round_won",
            userName,
            _scores[userId],
            _scores[userId] == 1 ? string.Empty : "s");
    }

    private async Task EndGame()
    {
        Cancel();

        var resultViewModel = new GuessingGameResultViewModel
        {
            Culture = Context.Culture,
            Scores = _scores
        };
        var template = await _templatesManager.GetTemplateAsync("GuessingGame/GuessingGameResult", resultViewModel);
        Context.SendHtml(template.RemoveNewlines());
    }

    public void Cancel()
    {
        OnEnd();
        CancelTimer();
    }

    protected abstract void OnGameStart();

    protected abstract Task OnTurnStart();
}