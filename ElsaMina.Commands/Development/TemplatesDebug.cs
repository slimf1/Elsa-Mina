using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Development;

public class TemplatesDebug : ICommand
{
    public static string Name => "templates";
    public static IEnumerable<string> Aliases => new[] { "templates-debug", "templatedebug" };
    public bool IsAllowedInPm => true;
    public bool IsWhitelistOnly => true;

    private readonly ITemplatesManager _templatesManager;

    public TemplatesDebug(ITemplatesManager templatesManager)
    {
        _templatesManager = templatesManager;
    }

    public async Task Run(IContext context)
    {
        var parts = context.Target.Split(",");
        var templateName = parts[0].Trim();
        var arguments = parts[1].Trim().ParseArguments();
        var template = await _templatesManager.GetTemplate(templateName, arguments);

        context.SendHtmlPage("test", template.RemoveNewlines());
    }
}