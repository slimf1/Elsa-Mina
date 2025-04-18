﻿using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Development.Commands;

[NamedCommand("allcommands", Aliases = ["all-commands", "commands"])]
public class AllCommands : Command
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly ITemplatesManager _templatesManager;

    public AllCommands(ICommandExecutor commandExecutor,
        ITemplatesManager templatesManager)
    {
        _commandExecutor = commandExecutor;
        _templatesManager = templatesManager;
    }

    public override bool IsAllowedInPrivateMessage => true;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var template = await _templatesManager.GetTemplateAsync("Development/Commands/CommandList", new CommandListViewModel
        {
            Commands = _commandExecutor.GetAllCommands().Where(command => !command.IsHidden),
            Culture = context.Culture
        });

        context.ReplyHtmlPage("all-commands", template.RemoveNewlines());
    }
}