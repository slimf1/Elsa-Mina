using ElsaMina.Core.Services.Battles.Dtos;
using ElsaMina.Logging;
using Newtonsoft.Json;

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

        if (parts[1] == "start")
        {
            result = new BattleMessageResult(BattleMessageType.BattleStarted);
            return true;
        }

        if (parts[1] is "win" or "tie")
        {
            context.IsBattleOver = true;
            var winnerName = parts[1] == "win" && parts.Length >= 3 ? parts[2] : null;
            var isTie = parts[1] == "tie";
            result = new BattleMessageResult(BattleMessageType.BattleEnded, winnerName, isTie);
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
            var battleState = JsonConvert.DeserializeObject<BattleStateDto>(requestJson);
            if (battleState == null)
            {
                return false;
            }

            context.Wait = battleState.Wait;
            context.TeamPreview = battleState.TeamPreview;
            context.ForceSwitchSlots = ParseForceSwitch(battleState);
            context.SidePokemon = ParseSidePokemon(battleState);
            context.ActiveSlots = ParseActiveSlots(battleState);

            result = new BattleMessageResult(BattleMessageType.RequestUpdated);
            return true;
        }
        catch (JsonException exception)
        {
            Log.Error(exception, "Failed to parse battle request");
            return false;
        }
    }

    private static List<bool> ParseForceSwitch(BattleStateDto root)
    {
        return root.ForceSwitch == null || root.ForceSwitch.Count == 0 ? [] : root.ForceSwitch;
    }

    private static List<BattlePokemonState> ParseSidePokemon(BattleStateDto root)
    {
        if (root.Side?.Pokemon == null || root.Side.Pokemon.Count == 0)
        {
            return [];
        }

        var results = new List<BattlePokemonState>(root.Side.Pokemon.Count);
        foreach (var pokemon in root.Side.Pokemon)
        {
            var condition = pokemon.Condition;
            var isFainted = !string.IsNullOrWhiteSpace(condition) &&
                            condition.Contains("fnt", StringComparison.OrdinalIgnoreCase);

            results.Add(new BattlePokemonState
            {
                IsActive = pokemon.Active,
                IsFainted = isFainted
            });
        }

        return results;
    }

    private static List<BattleActiveSlot> ParseActiveSlots(BattleStateDto root)
    {
        if (root.Active == null || root.Active.Count == 0)
        {
            return [];
        }

        var slots = new List<BattleActiveSlot>(root.Active.Count);
        foreach (var activeSlot in root.Active)
        {
            if (activeSlot?.Moves == null || activeSlot.Moves.Count == 0)
            {
                slots.Add(new BattleActiveSlot());
                continue;
            }

            var moves = new List<BattleMoveState>(activeSlot.Moves.Count);
            foreach (var move in activeSlot.Moves)
            {
                moves.Add(new BattleMoveState
                {
                    IsDisabled = move?.Disabled ?? false,
                    Type = "" // TODO : resource json
                });
            }

            slots.Add(new BattleActiveSlot { Moves = moves });
        }

        return slots;
    }
}
