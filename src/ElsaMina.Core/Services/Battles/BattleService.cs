using System.Collections.Concurrent;

namespace ElsaMina.Core.Services.Battles;

public class BattleService : IBattleService
{
    private readonly IBattleMessageParser _messageParser;
    private readonly IBattleDecisionService _decisionService;
    private readonly IBot _bot;
    private readonly ConcurrentDictionary<string, BattleContext> _contexts = new();

    public BattleService(IBattleMessageParser messageParser, IBattleDecisionService decisionService, IBot bot)
    {
        _messageParser = messageParser;
        _decisionService = decisionService;
        _bot = bot;
    }

    public Task HandleMessageAsync(string[] parts, string roomId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roomId) || !roomId.StartsWith("battle-"))
        {
            return Task.CompletedTask;
        }

        var context = _contexts.GetOrAdd(roomId, id => new BattleContext(id));
        if (!_messageParser.TryApplyMessage(parts, roomId, context, out var result))
        {
            return Task.CompletedTask;
        }

        if (result.Type == BattleMessageType.BattleEnded)
        {
            _bot.Say(roomId, "/leave");
            _contexts.TryRemove(roomId, out _);
            return Task.CompletedTask;
        }

        if (result.Type == BattleMessageType.RequestUpdated &&
            _decisionService.TryGetDecision(context, out var decision))
        {
            var command = BuildCommand(decision);
            if (!string.IsNullOrWhiteSpace(command))
            {
                _bot.Say(roomId, command);
            }
        }

        return Task.CompletedTask;
    }

    private static string BuildCommand(BattleDecision decision)
    {
        return decision.Type switch
        {
            BattleDecisionType.TeamPreview => $"/team {decision.Choices[0]}",
            BattleDecisionType.Switch => $"/choose {string.Join(", ", decision.Choices.Select(index => $"switch {index}"))}",
            BattleDecisionType.Move => $"/choose {string.Join(", ", decision.Choices.Select(index => $"move {index}"))}",
            _ => string.Empty
        };
    }
}
