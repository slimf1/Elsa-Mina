namespace ElsaMina.Core.Services.Battles.Strategies;

public class CalcBasedBattleDecisionService : IBattleDecisionService
{
    public bool TryGetDecision(BattleContext context, out BattleDecision decision)
    {
        decision = null;
        return false;
    }
}
