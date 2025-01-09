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
    public virtual string[] AllowedRooms => [];

    public void ReplyLocalizedHelpMessage(IContext context, params object[] formatArguments)
    {
        context.Reply(context.GetString(HelpMessageKey, formatArguments));
    }

    public virtual Task OnBotStartUp()
    {
        return Task.CompletedTask;
    }

    public async Task Call(IContext context)
    {
        if (IsPrivateMessageOnly && !context.IsPrivateMessage)
        {
            return;
        }

        if (context.IsPrivateMessage && !(IsAllowedInPrivateMessage || IsPrivateMessageOnly))
        {
            return;
        }

        if (IsWhitelistOnly && !context.IsSenderWhitelisted)
        {
            return;
        }

        if (!context.HasSufficientRank(RequiredRank))
        {
            return;
        }

        if (AllowedRooms.Length > 0 && !AllowedRooms.Contains(context.RoomId))
        {
            return;
        }

        await Run(context);
    }

    public abstract Task Run(IContext context);

    private void InitializeNameAndAliasesFromAttribute()
    {
        var commandAttribute = GetType().GetCommandAttribute();
        Name = commandAttribute?.Name ?? string.Empty;
        Aliases = commandAttribute?.Aliases ?? [];
    }
}