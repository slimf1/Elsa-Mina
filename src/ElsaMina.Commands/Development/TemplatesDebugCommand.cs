using ElsaMina.Commands.GuessingGame;
using ElsaMina.Commands.Profile;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Development;

[NamedCommand("templates", Aliases = ["templates-debug", "templatedebug"])]
public class TemplatesDebugCommand : DevelopmentCommand
{
    private readonly ITemplatesManager _templatesManager;

    public TemplatesDebugCommand(ITemplatesManager templatesManager)
    {
        _templatesManager = templatesManager;
    }
    
    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var parts = context.Target.Split(",");
        var templateName = parts[0].Trim();
        LocalizableViewModel model = templateName switch
        {
            "Profile/Profile" => new ProfileViewModel
            {
                UserId = parts[1],
                UserName = parts[2],
                Avatar = parts[3],
                Culture = context.Culture
            },
            "GuessingGame/GuessingGameResult" => new GuessingGameResultViewModel
            {
                Culture = context.Culture,
                Scores = new Dictionary<string, int>
                {
                    ["speks"] = 14,
                    ["morsay"] = 12,
                    ["thylane"] = 7,
                    ["lionyx"] = 1
                }
            },
            _ => null
        };
        
        var template = await _templatesManager.GetTemplateAsync(templateName, model);
        context.ReplyHtmlPage($"debug-template-{templateName}", template.RemoveNewlines());
    }
}