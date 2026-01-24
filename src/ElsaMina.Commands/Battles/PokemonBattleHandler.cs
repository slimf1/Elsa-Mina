using System.Text.Json;
using ElsaMina.Core;
using ElsaMina.Core.Handlers;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Logging;

namespace ElsaMina.Commands.Battles;

public class PokemonBattleHandler : Handler
{
    private readonly IBot _bot;
    private readonly IRandomService _randomService;

    public PokemonBattleHandler(IBot bot, IRandomService randomService)
    {
        _bot = bot;
        _randomService = randomService;
    }

    public override Task HandleReceivedMessageAsync(string[] parts, string roomId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roomId) || !roomId.StartsWith("battle-"))
        {
            return Task.CompletedTask;
        }

        if (parts.Length >= 2 && (parts[1] == "win" || parts[1] == "tie"))
        {
            _bot.Say(roomId, "/leave");
            return Task.CompletedTask;
        }

        if (parts.Length < 3 || parts[1] != "request")
        {
            return Task.CompletedTask;
        }

        var requestJson = parts[2];
        if (string.IsNullOrWhiteSpace(requestJson) || requestJson == "null")
        {
            return Task.CompletedTask;
        }

        try
        {
            using var document = JsonDocument.Parse(requestJson);
            var root = document.RootElement;

            if (root.TryGetProperty("wait", out var waitElement) &&
                waitElement.ValueKind == JsonValueKind.True)
            {
                return Task.CompletedTask;
            }

            if (root.TryGetProperty("teamPreview", out var teamPreviewElement) &&
                teamPreviewElement.ValueKind == JsonValueKind.True &&
                TryPickTeamPreview(root, out var teamChoice))
            {
                _bot.Say(roomId, $"/team {teamChoice}");
                return Task.CompletedTask;
            }

            if (TryBuildForceSwitchChoice(root, out var switchChoices))
            {
                var choice = string.Join(", ", switchChoices.Select(index => $"switch {index}"));
                _bot.Say(roomId, $"/choose {choice}");
                return Task.CompletedTask;
            }

            if (TryBuildMoveChoice(root, out var moveChoices))
            {
                var choice = string.Join(", ", moveChoices.Select(index => $"move {index}"));
                _bot.Say(roomId, $"/choose {choice}");
            }
        }
        catch (JsonException exception)
        {
            Log.Error(exception, "Failed to parse battle request");
        }

        return Task.CompletedTask;
    }

    private bool TryPickTeamPreview(JsonElement root, out int teamChoice)
    {
        teamChoice = 0;
        if (!root.TryGetProperty("side", out var sideElement) ||
            !sideElement.TryGetProperty("pokemon", out var pokemonElement) ||
            pokemonElement.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        var total = pokemonElement.GetArrayLength();
        if (total <= 0)
        {
            return false;
        }

        teamChoice = _randomService.NextInt(1, total + 1);
        return true;
    }

    private bool TryBuildForceSwitchChoice(JsonElement root, out List<int> choices)
    {
        choices = [];
        if (!root.TryGetProperty("forceSwitch", out var forceSwitchElement))
        {
            return false;
        }

        var forceSwitchSlots = new List<bool>();
        if (forceSwitchElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var element in forceSwitchElement.EnumerateArray())
            {
                forceSwitchSlots.Add(element.ValueKind == JsonValueKind.True);
            }
        }
        else if (forceSwitchElement.ValueKind == JsonValueKind.True)
        {
            forceSwitchSlots.Add(true);
        }

        if (forceSwitchSlots.All(slot => !slot))
        {
            return false;
        }

        var candidates = GetSwitchCandidates(root);
        if (candidates.Count == 0)
        {
            return false;
        }

        foreach (var _ in forceSwitchSlots.Where(slot => slot))
        {
            if (candidates.Count == 0)
            {
                return false;
            }

            var choice = _randomService.RandomElement(candidates);
            choices.Add(choice);
            candidates.Remove(choice);
        }

        return choices.Count > 0;
    }

    private List<int> GetSwitchCandidates(JsonElement root)
    {
        var candidates = new List<int>();
        if (!root.TryGetProperty("side", out var sideElement) ||
            !sideElement.TryGetProperty("pokemon", out var pokemonElement) ||
            pokemonElement.ValueKind != JsonValueKind.Array)
        {
            return candidates;
        }

        var index = 1;
        foreach (var pokemon in pokemonElement.EnumerateArray())
        {
            var isActive = pokemon.TryGetProperty("active", out var activeElement) &&
                           activeElement.ValueKind == JsonValueKind.True;
            var condition = pokemon.TryGetProperty("condition", out var conditionElement)
                ? conditionElement.GetString()
                : null;
            var isFainted = condition != null &&
                            condition.Contains("fnt", StringComparison.OrdinalIgnoreCase);

            if (!isActive && !isFainted)
            {
                candidates.Add(index);
            }

            index++;
        }

        return candidates;
    }

    private bool TryBuildMoveChoice(JsonElement root, out List<int> choices)
    {
        choices = [];
        if (!root.TryGetProperty("active", out var activeElement) ||
            activeElement.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        foreach (var activeSlot in activeElement.EnumerateArray())
        {
            if (!activeSlot.TryGetProperty("moves", out var movesElement) ||
                movesElement.ValueKind != JsonValueKind.Array)
            {
                return false;
            }

            var availableMoves = new List<int>();
            var moveIndex = 1;
            foreach (var move in movesElement.EnumerateArray())
            {
                var isDisabled = move.TryGetProperty("disabled", out var disabledElement) &&
                                 disabledElement.ValueKind == JsonValueKind.True;
                if (!isDisabled)
                {
                    availableMoves.Add(moveIndex);
                }

                moveIndex++;
            }

            if (availableMoves.Count == 0)
            {
                return false;
            }

            choices.Add(_randomService.RandomElement(availableMoves));
        }

        return choices.Count > 0;
    }
}
