using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Logging;
using Lusamine.DamageCalc;
using Lusamine.DamageCalc.Data;

namespace ElsaMina.Core.Services.Battles.Strategies;

public class CalcBasedBattleDecisionService : IBattleDecisionService
{
    private static readonly IGeneration Generation = DataIndex.Create(9);

    private readonly IRandomService _randomService;

    public CalcBasedBattleDecisionService(IRandomService randomService)
    {
        _randomService = randomService;
    }

    public bool TryGetDecision(BattleContext context, out BattleDecision decision)
    {
        decision = null;

        if (context.IsBattleOver || context.Wait)
        {
            return false;
        }

        if (context.TeamPreview && context.SidePokemon.Count > 0)
        {
            var choice = _randomService.NextInt(1, context.SidePokemon.Count + 1);
            decision = new BattleDecision(BattleDecisionType.TeamPreview, [choice]);
            return true;
        }

        if (context.ForceSwitchSlots.Any(slot => slot))
        {
            var choices = BuildSwitchChoices(context);
            if (choices.Count == 0)
            {
                return false;
            }

            decision = new BattleDecision(BattleDecisionType.Switch, choices);
            return true;
        }

        if (context.ActiveSlots.Count > 0)
        {
            var (choices, useTerastallize) = BuildMoveChoices(context);
            if (choices.Count == 0)
            {
                return false;
            }

            decision = new BattleDecision(BattleDecisionType.Move, choices, useTerastallize);
            return true;
        }

        return false;
    }

    private (List<int> choices, bool useTerastallize) BuildMoveChoices(BattleContext context)
    {
        var choices = new List<int>();
        var useTerastallize = false;

        for (var slotIndex = 0; slotIndex < context.ActiveSlots.Count; slotIndex++)
        {
            var slot = context.ActiveSlots[slotIndex];
            if (slot.Moves.Count == 0)
            {
                Log.Debug("No moves for slot {SlotIndex}", slotIndex);
                return ([], false);
            }

            var availableMoves = GetAvailableMoveIndices(slot);
            if (availableMoves.Count == 0)
            {
                Log.Debug("No available moves for slot {SlotIndex}", slotIndex);
                return ([], false);
            }

            var opponent = context.ActiveOpponent;
            var activePokemon = context.SidePokemon.FirstOrDefault(p => p.IsActive);

            if (opponent == null || activePokemon == null ||
                !TryBuildOurPokemon(activePokemon, out var calcAttacker) ||
                !TryBuildOpponentPokemon(opponent, out var calcDefender))
            {
                Log.Debug("Failed to build calc Pokemon for slot {SlotIndex} => random", slotIndex);
                choices.Add(_randomService.RandomElement(availableMoves));
                continue;
            }

            var (bestMoveIndex, bestScore) = ScoreMoves(slot, availableMoves, calcAttacker, calcDefender);

            // Check if Terastallizing improves the outcome
            if (!string.IsNullOrEmpty(slot.CanTerastallize) &&
                TryBuildOurPokemon(activePokemon, out var teraAttacker))
            {
                teraAttacker.TeraType = slot.CanTerastallize;
                var (teraBestIndex, teraScore) = ScoreMoves(slot, availableMoves, teraAttacker, calcDefender);
                if (teraScore > bestScore)
                {
                    bestMoveIndex = teraBestIndex;
                    useTerastallize = true;
                }
            }

            choices.Add(bestMoveIndex);
        }

        return (choices, useTerastallize);
    }

    private List<int> BuildSwitchChoices(BattleContext context)
    {
        var candidates = GetSwitchCandidates(context);
        if (candidates.Count == 0)
        {
            return [];
        }

        var choices = new List<int>();
        var remainingCandidates = new List<int>(candidates);

        foreach (var _ in context.ForceSwitchSlots.Where(slot => slot))
        {
            if (remainingCandidates.Count == 0)
            {
                return [];
            }

            var choice = PickBestSwitchIn(context, remainingCandidates);
            choices.Add(choice);
            remainingCandidates.Remove(choice);
        }

        return choices;
    }

    private int PickBestSwitchIn(BattleContext context, List<int> candidateIndices)
    {
        var opponent = context.ActiveOpponent;

        if (opponent == null || string.IsNullOrEmpty(opponent.LastUsedMove) ||
            !TryBuildOpponentPokemon(opponent, out var calcOpponent))
        {
            return _randomService.RandomElement(candidateIndices);
        }

        var bestIndex = candidateIndices[0];
        var bestDamageTakenPercent = double.MaxValue;

        foreach (var candidateIndex in candidateIndices)
        {
            var candidate = context.SidePokemon[candidateIndex - 1];
            if (!TryBuildOurPokemon(candidate, out var calcCandidate))
            {
                continue;
            }

            try
            {
                var incomingMove = new Move(Generation, opponent.LastUsedMove);
                var result = Calc.Calculate(Generation, calcOpponent, calcCandidate, incomingMove, null);
                var (_, maxDamage) = result.Range();
                var damageTakenPercent = (double)maxDamage / calcCandidate.MaxHP(false);

                if (damageTakenPercent < bestDamageTakenPercent)
                {
                    bestDamageTakenPercent = damageTakenPercent;
                    bestIndex = candidateIndex;
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Failed to evaluate switch-in for {Ident}", candidate.Ident);
            }
        }

        return bestIndex;
    }

    private static (int moveIndex, double score) ScoreMoves(
        BattleActiveSlot slot,
        List<int> availableMoveIndices,
        Pokemon attacker,
        Pokemon defender)
    {
        var bestIndex = availableMoveIndices[0];
        var bestScore = double.MinValue;

        foreach (var moveIndex in availableMoveIndices)
        {
            var move = slot.Moves[moveIndex - 1];
            try
            {
                var calcMove = new Move(Generation, move.Name);
                var result = Calc.Calculate(Generation, attacker, defender, calcMove, null);
                var score = ComputeMoveScore(result, defender);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = moveIndex;
                }
            }
            catch
            {
                // Move not in dex or produces no damage (status move) — skip
            }
        }

        return (bestIndex, bestScore);
    }

    private static double ComputeMoveScore(Result result, Pokemon defender)
    {
        var (koChance, nHko, _) = result.Kochance(false);
        var (_, maxDamage) = result.Range();
        var maxHp = defender.MaxHP(false);

        // Primary: fewest hits to KO (OHKO > 2HKO > ...), secondary: KO chance, tertiary: raw damage %
        var nkoScore = nHko > 0 ? 1000.0 / nHko : 0.0;
        var damagePercent = maxHp > 0 ? (double)maxDamage / maxHp : 0.0;
        return nkoScore + koChance * 100.0 + damagePercent;
    }

    private static bool TryBuildOurPokemon(BattlePokemonState state, out Pokemon pokemon)
    {
        pokemon = null;
        var species = ExtractSpeciesFromDetails(state.Details);
        var level = ExtractLevelFromDetails(state.Details);

        try
        {
            pokemon = new Pokemon(Generation, species, new State.Pokemon
            {
                Level = level,
                Item = string.IsNullOrEmpty(state.Item) ? null : state.Item,
                Ability = string.IsNullOrEmpty(state.Ability) ? null : state.Ability,
                TeraType = string.IsNullOrEmpty(state.Terastallized) ? null : state.Terastallized,
                Status = ExtractStatus(state.Condition),
                CurHP = state.CurrentHp > 0 ? state.CurrentHp : null
            });

            // Override with the exact in-battle stats from the request JSON
            pokemon.RawStats = new StatsTable
            {
                Hp = state.MaxHp,
                Atk = state.Stats.Atk,
                Def = state.Stats.Def,
                Spa = state.Stats.SpA,
                Spd = state.Stats.SpD,
                Spe = state.Stats.Spe
            };

            return true;
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Failed to build calc Pokemon for {Species}", species);
            return false;
        }
    }

    private static bool TryBuildOpponentPokemon(OpponentPokemonState state, out Pokemon pokemon)
    {
        pokemon = null;

        try
        {
            pokemon = new Pokemon(Generation, state.Species, new State.Pokemon
            {
                Level = state.Level,
                Status = string.IsNullOrEmpty(state.Status) ? null : state.Status,
                Boosts = BuildBoostsInput(state.Boosts)
            });

            // Apply tracked HP percentage — derive actual HP from computed max
            var maxHp = pokemon.MaxHP(false);
            pokemon.OriginalCurHP = (int)Math.Max(1, Math.Round(maxHp * state.HpPercent / 100.0));

            return true;
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Failed to build calc Pokemon for opponent {Species}", state.Species);
            return false;
        }
    }

    private static StatsTableInput BuildBoostsInput(Dictionary<string, int> boosts)
    {
        return new StatsTableInput
        {
            Atk = GetBoost(boosts, "atk"),
            Def = GetBoost(boosts, "def"),
            Spa = GetBoost(boosts, "spa"),
            Spd = GetBoost(boosts, "spd"),
            Spe = GetBoost(boosts, "spe")
        };
    }

    private static int? GetBoost(Dictionary<string, int> boosts, string key) =>
        boosts.TryGetValue(key, out var value) && value != 0 ? value : null;

    private static string ExtractSpeciesFromDetails(string details)
    {
        var commaIndex = details.IndexOf(',');
        return commaIndex < 0 ? details : details[..commaIndex];
    }

    private static int ExtractLevelFromDetails(string details)
    {
        foreach (var token in details.Split(", ").AsSpan(1))
        {
            if (token.StartsWith('L') && int.TryParse(token.AsSpan(1), out var level))
            {
                return level;
            }
        }

        return 100;
    }

    private static string ExtractStatus(string condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
        {
            return null;
        }

        var spaceIndex = condition.IndexOf(' ');
        if (spaceIndex < 0)
        {
            return null;
        }

        var statusPart = condition[(spaceIndex + 1)..];
        return statusPart is "fnt" or "" ? null : statusPart;
    }

    private static List<int> GetAvailableMoveIndices(BattleActiveSlot slot)
    {
        var available = new List<int>();
        for (var index = 0; index < slot.Moves.Count; index++)
        {
            var move = slot.Moves[index];
            if (move.Name == "Recharge" || (!move.IsDisabled && move.Pp > 0))
            {
                available.Add(index + 1);
            }
        }

        return available;
    }

    private static List<int> GetSwitchCandidates(BattleContext context)
    {
        var candidates = new List<int>();
        for (var index = 0; index < context.SidePokemon.Count; index++)
        {
            var pokemon = context.SidePokemon[index];
            if (!pokemon.IsActive && !pokemon.IsFainted)
            {
                candidates.Add(index + 1);
            }
        }

        return candidates;
    }
}
