﻿using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.GuessingGame;

public abstract class GuessingGame : Game
{
    private const int DEFAULT_TURNS_COUNT = 10;
    private const int MIN_LENGTH_FOR_AUTOCORRECT = 8;
    private static readonly TimeSpan TURN_COOLDOWN = TimeSpan.FromSeconds(15);

    private readonly ITemplatesManager _templatesManager;
    private readonly IConfigurationManager _configurationManager;
    private CancellationTokenSource _cancellationTokenSource;

    private readonly Dictionary<string, int> _scores = new();
    private int _currentTurn;
    private bool _hasRoundBeenWon;
    private bool _ended;

    protected GuessingGame(ITemplatesManager templatesManager,
        IConfigurationManager configurationManager)
    {
        _templatesManager = templatesManager;
        _configurationManager = configurationManager;
    }
    
    protected IEnumerable<string> CurrentValidAnswers { get; set; } = [];

    public int TurnsCount { get; set; } = DEFAULT_TURNS_COUNT;
    public IRoom Room { get; set; }

    public void Start()
    {
        OnGameStart();
        InitializeNextTurn();
    }

    private void InitializeNextTurn()
    {
        _currentTurn++;
        Context.ReplyLocalizedMessage("guessing_game_turn_count", _currentTurn);
        OnTurnStart();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(async () =>
        {
            await Task.Delay(TURN_COOLDOWN, _cancellationTokenSource.Token);
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

    protected virtual void OnGameStart()
    {
    }

    protected abstract void OnTurnStart();
}