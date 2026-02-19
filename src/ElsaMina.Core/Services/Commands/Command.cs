using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Services.Commands;

public abstract class Command : ICommand
{
    protected Command()
    {
        InitializeNameAndAliasesFromAttribute();
    }

    public string Name { get; private set; }
    public IEnumerable<string> Aliases { get; private set; }
    public virtual bool IsAllowedInPrivateMessage => false;
    public virtual bool IsWhitelistOnly => false;
    public virtual bool IsPrivateMessageOnly => false;
    public virtual Rank RequiredRank => Rank.Admin; // todo remplacer ça par une méthode virtual "CanRun" avec contexte
    public virtual string HelpMessageKey => string.Empty;
    public virtual bool IsHidden => false;
    public virtual IEnumerable<string> RoomRestriction => [];

    protected void ReplyLocalizedHelpMessage(IContext context, bool rankAware = false)
    {
        context.Reply(context.GetString(HelpMessageKey), rankAware: rankAware);
    }

    public abstract Task RunAsync(IContext context, CancellationToken cancellationToken = default);

    private void InitializeNameAndAliasesFromAttribute()
    {
        var commandAttribute = GetType().GetCommandAttribute();
        Name = commandAttribute?.Name ?? string.Empty;
        Aliases = commandAttribute?.Aliases ?? [];
    }
}