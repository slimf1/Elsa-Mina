using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Showdown.Searching;

[NamedCommand("search")]
public class SearchCommand : Command
{
    public override Rank RequiredRank => Rank.Regular;

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var format = context.Target?.Trim();
        if (string.IsNullOrWhiteSpace(format))
        {
            context.Reply("Usage: /search <format>");
            return Task.CompletedTask;
        }

        context.SendMessageIn(context.RoomId, $"/search {format}");
        return Task.CompletedTask;
    }
}
