using System.Reflection;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Commands;

public interface ICommand
{
    public static virtual string Name => string.Empty;
    public static virtual IEnumerable<string> Aliases => Enumerable.Empty<string>();
    public bool IsAllowedInPm => false;
    public bool IsWhitelistOnly => false;
    public bool IsPrivateMessageOnly => false;
    public char RequiredRank => '&';
    public string HelpMessageKey => "";
    public bool IsHidden => false;

    public sealed string CommandName
    {
        get
        {
            try
            {
                return (string)((PropertyInfo)GetType()
                    .GetMember("Name")
                    .GetValue(0))?
                    .GetMethod?
                    .Invoke(null, null);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
    
    public sealed IEnumerable<string> CommandAliases
    {
        get
        {
            try
            {
                return (IEnumerable<string>)((PropertyInfo)GetType()
                        .GetMember("Aliases")
                        .GetValue(0))?
                    .GetMethod?
                    .Invoke(null, null);
            }
            catch (Exception)
            {
                return Enumerable.Empty<string>();
            }
        }
    }

    protected sealed void ReplyLocalizedHelpMessage(IContext context, params object[] formatArguments)
    {
        context.Reply(context.GetString(HelpMessageKey, formatArguments));
    }

    public sealed async Task Call(IContext context)
    {
        if (IsPrivateMessageOnly && !context.IsPm)
        {
            return;
        }

        if (context.IsPm && !(IsAllowedInPm || IsPrivateMessageOnly))
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

        await Run(context);
    }

    protected Task Run(IContext context);
}