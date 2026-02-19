using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Services.Commands;

public interface ICommand
{
    public string Name { get; }
    public IEnumerable<string> Aliases { get; }
    public bool IsAllowedInPrivateMessage { get; }
    public bool IsWhitelistOnly { get; }
    public bool IsPrivateMessageOnly { get; }
    public Rank RequiredRank { get; }
    public string HelpMessageKey { get; }
    public bool IsHidden { get; }
    public IEnumerable<string> RoomRestriction { get; }

    Task RunAsync(IContext context, CancellationToken cancellationToken = default);
}