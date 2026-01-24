namespace ElsaMina.Core.Services.Battles;

public class BattleDecision
{
    public BattleDecision(BattleDecisionType type, List<int> choices)
    {
        Type = type;
        Choices = choices;
    }

    public BattleDecisionType Type { get; }
    public List<int> Choices { get; }
}
