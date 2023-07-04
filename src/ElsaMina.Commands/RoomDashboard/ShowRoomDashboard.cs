using System.Globalization;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templating;
using ElsaMina.Core.Templates.LanguageSelect;
using ElsaMina.Core.Templates.RoomDashboard;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.RoomDashboard;

public class ShowRoomDashboard : BaseCommand<ShowRoomDashboard>
{
    public static string Name => "room-dashboard";
    public bool IsPrivateMessageOnly => true;
    public bool IsWhitelistOnly => true; // todo : seul un mec authed sur la room peut

    private readonly IConfigurationManager _configurationManager;
    private readonly IResourcesService _resourcesService;
    private readonly IRoomsManager _roomsManager;
    private readonly IRepository<RoomParameters, string> _roomParametersRepository;
    private readonly ITemplatesManager _templatesManager;

    public ShowRoomDashboard(IConfigurationManager configurationManager,
        IResourcesService resourcesService,
        IRoomsManager roomsManager,
        IRepository<RoomParameters, string> roomParametersRepository,
        ITemplatesManager templatesManager)
    {
        _configurationManager = configurationManager;
        _resourcesService = resourcesService;
        _roomsManager = roomsManager;
        _roomParametersRepository = roomParametersRepository;
        _templatesManager = templatesManager;
    }

    public override async Task Run(IContext context)
    {
        var roomId = context.Target.Trim().ToLower();
        if (string.IsNullOrEmpty(roomId))
        {
            roomId = context.RoomId;
        }

        var room = _roomsManager.GetRoom(roomId);

        if (room == null)
        {
            context.ReplyLocalizedMessage("dashboard_room_doesnt_exist", roomId);
            return;
        }

        var roomParameters = await _roomParametersRepository.GetByIdAsync(roomId);
        if (roomParameters == null)
        {
            context.SendHtmlPage("dashboard-error", "<p>Could not find room parameters somehow</p>");
            return;
        }
        
        if (context.IsPm)
        {
            context.Locale = new CultureInfo(roomParameters.Locale
                                             ?? _configurationManager.Configuration.DefaultLocaleCode);
        }

        var viewModel = new RoomDashboardViewModel
        {
            BotName = _configurationManager.Configuration.Name,
            Trigger = _configurationManager.Configuration.Trigger,
            RoomParameters = roomParameters,
            RoomName = room.Name,
            Culture = context.Locale,
            LanguageSelectModel = new LanguagesSelectViewModel
            {
                Name = "locale",
                Id = "locale",
                Cultures = _resourcesService.SupportedLocales,
                Culture = context.Locale
            }
        };
        var template = await _templatesManager.GetTemplate("RoomDashboard/RoomDashboard", viewModel);
        
        context.SendHtmlPage($"{roomId}dashboard", template.RemoveNewlines());
    }
}
