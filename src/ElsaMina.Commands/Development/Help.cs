﻿using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;

namespace ElsaMina.Commands.Development;

public class Help : Command<Help>, INamed
{
    public static string Name => "help";
    public static List<string> Aliases => ["about"];
    public override bool IsAllowedInPm => true;
    public override char RequiredRank => '+';

    public override Task Run(IContext context)
    {
        context.ReplyLocalizedMessage("help");
        return Task.CompletedTask;
    }
}