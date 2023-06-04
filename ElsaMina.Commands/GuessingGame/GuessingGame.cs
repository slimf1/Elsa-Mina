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
    private readonly int _maxScore;

    private readonly Dictionary<string, int> _scores = new();
    private int _currentTurn;
    private IEnumerable<string> _currentAnswer = Enumerable.Empty<string>();
    private bool _roundWon;
    private bool _ended = false;

    public GuessingGame(IContext context,
        ITemplatesManager templatesManager,
        int turnsCount,
        int maxScore) : base(context)
    {
        _templatesManager = templatesManager;
        _turnsCount = turnsCount;
        _maxScore = maxScore;
    }

    async Task Start()
    {
        SendInitMessage();
        await InitializeNextTurn();
    }

    protected virtual void SendInitMessage()
    {
        Context.ReplyLocalizedMessage("guessing_game_default_init_message");
    }

    private async Task InitializeNextTurn()
    {
        _currentTurn++;
        Context.ReplyLocalizedMessage("guessing_game_turn_count", _currentTurn);
        await SetupTurn();
        await Task.Run(async () =>
        {
            await Task.Delay(SECONDS_BETWEEN_TURNS * 1000);
            await OnTurnEnd();
        });
    }

    private async Task OnTurnEnd()
    {
        if (!_roundWon)
        {
            Context.ReplyLocalizedMessage("guessing_game_answer_not_found",
                string.Join(", ", _currentAnswer));
        }

        _roundWon = false;
        if (_currentTurn >= _turnsCount || _ended)
        {
            await EndGame();
        }
    }

    private async Task EndGame()
    {
        var resultViewModel = new GuessingGameResultViewModel
        {
            Culture = Context.Locale.Name,
            Scores = _scores
        };
        var template = await _templatesManager.GetTemplate("GuessingGame/GuessingGameResult", resultViewModel);
    }

    private void OnAnswer(string userName, string answer)
    {
        if (_roundWon)
        {
            return;
        }

        var userId = userName.ToLowerAlphaNum();

        foreach (var validAnswer in _currentAnswer)
        {
            // TODO : string distance
            if (validAnswer.ToLower().Trim() == answer.ToLower().Trim())
            {
                _roundWon = true;
                // TODO: value => object with name and id
            }
        }
    }

    protected abstract Task SetupTurn();
}