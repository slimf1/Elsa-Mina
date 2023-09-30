using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Templates.Commands;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Development;

public class AllCommands : BaseCommand<AllCommands>, INamed
{
    public static string Name => "allcommands";
    public static IEnumerable<string> Aliases => new[] { "all-commands", "commands" };

    private readonly ICommandExecutor _commandExecutor;
    private readonly ITemplatesManager _templatesManager;

    public AllCommands(ICommandExecutor commandExecutor,
        ITemplatesManager templatesManager)
    {
        _commandExecutor = commandExecutor;
        _templatesManager = templatesManager;
    }
    
    public override bool IsAllowedInPm => true;

    public override async Task Run(IContext context)
    {
        var template = await _templatesManager.GetTemplate("Commands/CommandList", new CommandListViewModel
        {
            Commands = _commandExecutor.GetAllCommands().Where(command => !command.IsHidden),
            Culture = context.Locale
        });

        context.SendHtmlPage("all-commands", template.RemoveNewlines());
    }
}