namespace ElsaMina.Core.Services.Battles;

public interface IBattleDecisionService
{
    bool TryGetDecision(BattleContext context, out BattleDecision decision);
}
