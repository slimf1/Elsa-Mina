using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.GuessingGame;

public abstract class GuessingGame : Game
{
    private const int DEFAULT_TURNS_COUNT = 10;
    private const int SECONDS_BETWEEN_TURNS = 15;
    private const int MIN_LENGTH_FOR_AUTOCORRECT = 8;

    private readonly ITemplatesManager _templatesManager;
    private readonly IConfigurationManager _configurationManager;
    private CancellationTokenSource _cancellationTokenSource;

    private readonly Dictionary<string, int> _scores = new();
    private int _currentTurn;
    private bool _hasRoundBeenWon;
    private bool _ended;

    protected IEnumerable<string> CurrentValidAnswers { get; set; } = Enumerable.Empty<string>();

    protected GuessingGame(ITemplatesManager templatesManager,
        IConfigurationManager configurationManager)
    {
        _templatesManager = templatesManager;
        _configurationManager = configurationManager;
    }
    
    public int TurnsCount { get; set; } = DEFAULT_TURNS_COUNT;
    public IRoom Room { get; set; }

    public void Start()
    {
        SendInitMessage();
        InitializeNextTurn();
    }

    private void InitializeNextTurn()
    {
        _currentTurn++;
        Context.ReplyLocalizedMessage("guessing_game_turn_count", _currentTurn);
        SetupTurn();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(async () =>
        {
            await Task.Delay(SECONDS_BETWEEN_TURNS * 1000, _cancellationTokenSource.Token);
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
            await OnTurnEnd();
        }, _cancellationTokenSource.Token);
    }

    private async Task OnTurnEnd()
    {
        if (!_hasRoundBeenWon)
        {
            Context.ReplyLocalizedMessage("guessing_game_answer_not_found",
                string.Join(", ", CurrentValidAnswers.Distinct()));
        }

        _hasRoundBeenWon = false;
        if (_currentTurn >= TurnsCount || _ended)
        {
            await EndGame();
        }
        else
        {
            InitializeNextTurn();
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
                Text.LevenshteinDistance(validAnswer.ToLower(), answer.ToLower()) <= maxLevenshteinDistance))
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
        if (_cancellationTokenSource != null)
        {
            await _cancellationTokenSource.CancelAsync();
        }
        var resultViewModel = new GuessingGameResultViewModel
        {
            Culture = Context.Culture,
            Scores = _scores
        };
        var template = await _templatesManager.GetTemplate("GuessingGame/GuessingGameResult", resultViewModel);
        Context.Reply(template.RemoveNewlines());
        Room?.EndGame();
    }

    public override void Cancel()
    {
        base.Cancel();
        _ended = true;
        _cancellationTokenSource?.Cancel();
    }

    protected abstract void SendInitMessage();

    protected abstract void SetupTurn();
}