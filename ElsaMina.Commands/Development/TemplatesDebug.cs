using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Templates;
using ElsaMina.Core.Templates.LanguageSelect;
using ElsaMina.Core.Templates.Profile;
using ElsaMina.Core.Templates.RoomDashboard;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Development;

public class TemplatesDebug : ICommand
{
    public static string Name => "templates";
    public static IEnumerable<string> Aliases => new[] { "templates-debug", "templatedebug" };
    public bool IsAllowedInPm => true;
    public bool IsWhitelistOnly => true;

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

    public async Task Run(IContext context)
    {
        var parts = context.Target.Split(",");
        var templateName = parts[0].Trim();
        BaseViewModel model = templateName switch
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
                    Culture = context.Locale.Name
                },
                RoomParameters = new RoomParameters
                {
                    Id = "deez"
                }
            },
            _ => throw new ArgumentOutOfRangeException()
        };
        model.Culture = context.Locale.Name;
        
        var template = await _templatesManager.GetTemplate(templateName, model);
        context.SendHtmlPage($"debug-template-{templateName}", template.RemoveNewlines());
    }
}