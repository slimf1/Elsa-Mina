using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Templates.GuessingGame;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.GuessingGame;

public abstract class GuessingGame : Game
{
    private const int SECONDS_BETWEEN_TURNS = 16;

    private readonly ITemplatesManager _templatesManager;
    private readonly int _turnsCount;
    private CancellationTokenSource _cancellationTokenSource;

    private readonly Dictionary<string, int> _scores = new();
    private int CurrentTurn { get; set; }
    protected IEnumerable<string> CurrentValidAnswers { get; set; } = Enumerable.Empty<string>();
    private bool HasRoundBeenWon { get; set; }
    public bool Ended { get; set; }

    public GuessingGame(IContext context,
        ITemplatesManager templatesManager,
        int turnsCount) : base(context)
    {
        _templatesManager = templatesManager;
        _turnsCount = turnsCount;
    }

    public void Start()
    {
        SendInitMessage();
        InitializeNextTurn();
    }
    
    private void InitializeNextTurn()
    {
        CurrentTurn++;
        Context.ReplyLocalizedMessage("guessing_game_turn_count", CurrentTurn);
        SetupTurn();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(async () =>
        {
            await Task.Delay(SECONDS_BETWEEN_TURNS * 1000);
            await OnTurnEnd();
        }, _cancellationTokenSource.Token);
    }

    private async Task OnTurnEnd()
    {
        if (!HasRoundBeenWon)
        {
            Context.ReplyLocalizedMessage("guessing_game_answer_not_found",
                string.Join(", ", CurrentValidAnswers));
        }

        HasRoundBeenWon = false;
        if (CurrentTurn >= _turnsCount || Ended)
        {
            await EndGame();
        }
        else
        {
            InitializeNextTurn();
        }
    }

    private async Task EndGame()
    {
        _cancellationTokenSource?.Cancel();
        var resultViewModel = new GuessingGameResultViewModel
        {
            Culture = Context.Locale.Name,
            Scores = _scores
        };
        var template = await _templatesManager.GetTemplate("GuessingGame/GuessingGameResult", resultViewModel);
    }

    public void OnAnswer(string userName, string answer)
    {
        if (HasRoundBeenWon)
        {
            return;
        }

        var userId = userName.ToLowerAlphaNum();

        foreach (var validAnswer in CurrentValidAnswers)
        {
            // TODO : string distance
            if (validAnswer.ToLower().Trim() == answer.ToLower().Trim())
            {
                HasRoundBeenWon = true;
                if (!_scores.ContainsKey(userId))
                {
                    _scores[userId] = 0;
                }
                _scores[userId] += 1;
                Context.ReplyLocalizedMessage("guessing_game_round_won",
                    userName[1..], _scores[userId], _scores[userId] == 1 ? string.Empty : "s");
            }
        }
    }
    
    protected abstract void SendInitMessage();

    protected abstract void SetupTurn();
}