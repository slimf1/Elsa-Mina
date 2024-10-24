﻿using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;

namespace ElsaMina.Commands.Development;

[NamedCommand("help", Aliases = ["about"])]
public class Help : Command
{
    private readonly IVersionProvider _versionProvider;

    public Help(IVersionProvider versionProvider)
    {
        _versionProvider = versionProvider;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override char RequiredRank => '+';

    public override Task Run(IContext context)
    {
        context.ReplyLocalizedMessage("help", _versionProvider.Version);
        return Task.CompletedTask;
    }
}