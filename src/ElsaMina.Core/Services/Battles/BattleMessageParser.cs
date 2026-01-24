using System.Text.Json;
using ElsaMina.Logging;

namespace ElsaMina.Core.Services.Battles;

public class BattleMessageParser : IBattleMessageParser
{
    public bool TryApplyMessage(string[] parts, string roomId, BattleContext context, out BattleMessageResult result)
    {
        result = new BattleMessageResult(BattleMessageType.None);

        if (parts.Length < 2)
        {
            return false;
        }

        if (parts[1] is "win" or "tie")
        {
            context.IsBattleOver = true;
            result = new BattleMessageResult(BattleMessageType.BattleEnded);
            return true;
        }

        if (parts[1] != "request" || parts.Length < 3)
        {
            return false;
        }

        var requestJson = parts[2];
        if (string.IsNullOrWhiteSpace(requestJson) || requestJson == "null")
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(requestJson);
            var root = document.RootElement;

            context.Wait = root.TryGetProperty("wait", out var waitElement) &&
                           waitElement.ValueKind == JsonValueKind.True;
            context.TeamPreview = root.TryGetProperty("teamPreview", out var teamPreviewElement) &&
                                  teamPreviewElement.ValueKind == JsonValueKind.True;

            context.ForceSwitchSlots = ParseForceSwitch(root);
            context.SidePokemon = ParseSidePokemon(root);
            context.ActiveSlots = ParseActiveSlots(root);

            result = new BattleMessageResult(BattleMessageType.RequestUpdated);
            return true;
        }
        catch (JsonException exception)
        {
            Log.Error(exception, "Failed to parse battle request");
            return false;
        }
    }

    private static List<bool> ParseForceSwitch(JsonElement root)
    {
        if (!root.TryGetProperty("forceSwitch", out var forceSwitchElement))
        {
            return [];
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

        return forceSwitchSlots;
    }

    private static List<BattlePokemonState> ParseSidePokemon(JsonElement root)
    {
        if (!root.TryGetProperty("side", out var sideElement) ||
            !sideElement.TryGetProperty("pokemon", out var pokemonElement) ||
            pokemonElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var results = new List<BattlePokemonState>();
        foreach (var pokemon in pokemonElement.EnumerateArray())
        {
            var isActive = pokemon.TryGetProperty("active", out var activeElement) &&
                           activeElement.ValueKind == JsonValueKind.True;
            var condition = pokemon.TryGetProperty("condition", out var conditionElement)
                ? conditionElement.GetString()
                : null;
            var isFainted = condition != null &&
                            condition.Contains("fnt", StringComparison.OrdinalIgnoreCase);

            results.Add(new BattlePokemonState
            {
                IsActive = isActive,
                IsFainted = isFainted
            });
        }

        return results;
    }

    private static List<BattleActiveSlot> ParseActiveSlots(JsonElement root)
    {
        if (!root.TryGetProperty("active", out var activeElement) ||
            activeElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var slots = new List<BattleActiveSlot>();
        foreach (var activeSlot in activeElement.EnumerateArray())
        {
            if (!activeSlot.TryGetProperty("moves", out var movesElement) ||
                movesElement.ValueKind != JsonValueKind.Array)
            {
                slots.Add(new BattleActiveSlot());
                continue;
            }

            var moves = new List<BattleMoveState>();
            foreach (var move in movesElement.EnumerateArray())
            {
                var isDisabled = move.TryGetProperty("disabled", out var disabledElement) &&
                                 disabledElement.ValueKind == JsonValueKind.True;
                moves.Add(new BattleMoveState { IsDisabled = isDisabled });
            }

            slots.Add(new BattleActiveSlot { Moves = moves });
        }

        return slots;
    }
}
