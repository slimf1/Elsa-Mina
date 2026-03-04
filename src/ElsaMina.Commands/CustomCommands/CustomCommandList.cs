using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Commands.CustomCommands;

[NamedCommand("custom-command-list", Aliases =
[
    "added-command-list", "added-commands", "custom-commands",
    "addedcommands", "customcommands", "commandslist", "commandlist", "customcommandlist", "customs-list"
])]
public class CustomCommandList : Command
{
    private readonly IBotDbContextFactory _dbContextFactory;
    private readonly ITemplatesManager _templatesManager;

    public CustomCommandList(IBotDbContextFactory dbContextFactory, ITemplatesManager templatesManager)
    {
        _dbContextFactory = dbContextFactory;
        _templatesManager = templatesManager;
    }

    public override Rank RequiredRank => Rank.Voiced;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var addedCommands = await dbContext.AddedCommands
            .Where(command => command.RoomId == context.RoomId)
            .ToListAsync(cancellationToken);

        if (addedCommands.Count == 0)
        {
            context.ReplyLocalizedMessage("customcommandlist_no_commands");
            return;
        }

        var template = await _templatesManager.GetTemplateAsync("CustomCommands/CustomCommandList",
            new CustomCommandListViewModel
            {
                Culture = context.Culture,
                Commands = addedCommands
            });

        context.ReplyHtmlPage($"custom-commands-{context.RoomId}", template.RemoveNewlines());
    }
}