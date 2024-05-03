using ElsaMina.Commands.GuessingGame;
using ElsaMina.Commands.Profile;
using ElsaMina.Commands.RoomDashboard;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Development;

public class TemplatesDebug : DevelopmentCommand<TemplatesDebug>, INamed
{
    public static string Name => "templates";
    public static List<string> Aliases => ["templates-debug", "templatedebug"];

    private readonly ITemplatesManager _templatesManager;
    private readonly IResourcesService _resourcesService;
    private readonly IConfigurationManager _configurationManager;

    public TemplatesDebug(ITemplatesManager templatesManager,
        IResourcesService resourcesService,
        IConfigurationManager configurationManager)
    {
        _templatesManager = templatesManager;
        _resourcesService = resourcesService;
        _configurationManager = configurationManager;
    }
    
    public override async Task Run(IContext context)
    {
        var parts = context.Target.Split(",");
        var templateName = parts[0].Trim();
        LocalizableViewModel model = templateName switch
        {
            "Profile/Profile" => new ProfileViewModel
            {
                UserId = parts[1],
                UserName = parts[2],
                Avatar = parts[3]
            },
            "LanguageSelect/LanguagesSelect" => new LanguagesSelectViewModel
            {
                Name = "locale",
                Id = "locale",
                Cultures = _resourcesService.SupportedLocales
            },
            "RoomDashboard/RoomDashboard" => new RoomDashboardViewModel
            {
                BotName = _configurationManager.Configuration.Name,
                RoomName = context.RoomId,
                Trigger = _configurationManager.Configuration.Trigger,
                LanguageSelectModel = new LanguagesSelectViewModel
                {
                    Name = "locale",
                    Id = "locale",
                    Cultures = _resourcesService.SupportedLocales,
                    Culture = context.Culture
                },
                RoomParameters = new RoomParameters
                {
                    Id = "deez"
                }
            },
            "GuessingGame/GuessingGameResult" => new GuessingGameResultViewModel
            {
                Culture = context.Culture,
                Scores = new Dictionary<string, int>
                {
                    ["speks"] = 14,
                    ["morsay"] = 12,
                    ["thylane"] = 7,
                    ["lionyx"] = 1,
                }
            },
            _ => throw new ArgumentOutOfRangeException()
        };
        model.Culture = context.Culture;
        
        var template = await _templatesManager.GetTemplate(templateName, model);
        context.SendHtmlPage($"debug-template-{templateName}", template.RemoveNewlines());
    }
}