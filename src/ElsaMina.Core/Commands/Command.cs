using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Commands;

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
    public virtual Rank RequiredRank => Rank.Admin;
    public virtual string HelpMessageKey => string.Empty;
    public virtual bool IsHidden => false;
    public virtual IEnumerable<string> RoomRestriction => [];
    public virtual int Priority => 0;

    public void ReplyLocalizedHelpMessage(IContext context)
    {
        context.Reply(context.GetString(HelpMessageKey));
    }

    public abstract Task RunAsync(IContext context, CancellationToken cancellationToken = default);

    private void InitializeNameAndAliasesFromAttribute()
    {
        var commandAttribute = GetType().GetCommandAttribute();
        Name = commandAttribute?.Name ?? string.Empty;
        Aliases = commandAttribute?.Aliases ?? [];
    }

    public virtual void OnStart()
    {
    }

    public virtual void OnReconnect()
    {
    }

    public virtual void OnDisconnect()
    {
    }
}