using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.GuessingGame.Gatekeepers;

public class GatekeepersGame : GuessingGame
{
    public GatekeepersGame(ITemplatesManager templatesManager, IConfigurationManager configurationManager)
        : base(templatesManager, configurationManager)
    {
    }

    public override string Identifier => nameof(GatekeepersGame);

    protected override void OnGameStart()
    {
        throw new NotImplementedException();
    }

    protected override Task OnTurnStart()
    {
        throw new NotImplementedException();
    }

    protected override void OnTimerCountdown(TimeSpan remainingTime)
    {
        base.OnTimerCountdown(remainingTime);
    }
}