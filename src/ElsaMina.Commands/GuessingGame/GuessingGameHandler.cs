using ElsaMina.Core.Contexts;
using ElsaMina.Core.Handlers.DefaultHandlers;

namespace ElsaMina.Commands.GuessingGame;

public class GuessingGameHandler : ChatMessageHandler
{
    public GuessingGameHandler(IContextFactory contextFactory)
        : base(contextFactory)
    {
    }

    public override Task HandleMessage(IContext context)
    {
        if (context.Room?.Game is IGuessingGame guessingGame)
        {
            guessingGame.OnAnswer(context.Sender.Name, context.Message);
        }

        return Task.CompletedTask;
    }
}