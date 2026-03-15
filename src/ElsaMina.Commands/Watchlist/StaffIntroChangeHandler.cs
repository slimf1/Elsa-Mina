using ElsaMina.Core;
using ElsaMina.Core.Handlers;

namespace ElsaMina.Commands.Watchlist;

public class StaffIntroChangeHandler : Handler
{
    private readonly IBot _bot;

    public StaffIntroChangeHandler(IBot bot)
    {
        _bot = bot;
    }

    public override Task HandleReceivedMessageAsync(string[] parts, string roomId = null,
        CancellationToken cancellationToken = default)
    {
        if (parts.Length < 4 || parts[1] != "c")
        {
            return Task.CompletedTask;
        }

        var message = parts[3].Trim();
        if (message.StartsWith("/log") && message.Contains("changed the staffintro"))
        {
            _bot.Say(roomId, "/staffintro");
        }

        return Task.CompletedTask;
    }
}
