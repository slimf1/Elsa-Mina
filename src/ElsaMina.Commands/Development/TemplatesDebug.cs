using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Templates;
using ElsaMina.Core.Templates.GuessingGame;
using ElsaMina.Core.Templates.LanguageSelect;
using ElsaMina.Core.Templates.Profile;
using ElsaMina.Core.Templates.RoomDashboard;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Development;

public class TemplatesDebug : Command<TemplatesDebug>, INamed
{
    public static string Name => "templates";
    public static IEnumerable<string> Aliases => new[] { "templates-debug", "templatedebug" };

    private readonly ITemplatesManager _templatesManager;
    private readonly IResourcesService _resourcesService;
    private readonly IConfigurationManager _configurationManager;
    private readonly IRoomsManager _roomsManager;

    public TemplatesDebug(ITemplatesManager templatesManager,
        IResourcesService resourcesService,
        IConfigurationManager configurationManager,
        IRoomsManager roomsManager)
    {
        _templatesManager = templatesManager;
        _resourcesService = resourcesService;
        _configurationManager = configurationManager;
        _roomsManager = roomsManager;
    }
    
    public override bool IsAllowedInPm => true;
    public override bool IsWhitelistOnly => true;

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
                    Culture = context.Locale
                },
                RoomParameters = new RoomParameters
                {
                    Id = "deez"
                }
            },
            "GuessingGame/GuessingGameResult" => new GuessingGameResultViewModel
            {
                Culture = context.Locale,
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
        model.Culture = context.Locale;
        
        var template = await _templatesManager.GetTemplate(templateName, model);
        context.SendHtmlPage($"debug-template-{templateName}", template.RemoveNewlines());
    }
}