using System.Globalization;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Contexts;

public class DefaultContextProvider : IContextProvider
{
    private readonly IConfigurationManager _configurationManager;
    private readonly IResourcesService _resourcesService;
    private readonly IRoomsManager _roomsManager;

    public DefaultContextProvider(IConfigurationManager configurationManager,
        IResourcesService resourcesService,
        IRoomsManager roomsManager)
    {
        _configurationManager = configurationManager;
        _resourcesService = resourcesService;
        _roomsManager = roomsManager;
    }

    public IEnumerable<string> CurrentWhitelist => _configurationManager.Configuration.Whitelist;
    public string DefaultRoom => _configurationManager.Configuration.DefaultRoom;
    public CultureInfo DefaultCulture => new(_configurationManager.Configuration.DefaultLocaleCode);

    public string GetString(string key, CultureInfo culture)
    {
        var localizedString = _resourcesService.GetString(key, culture ?? DefaultCulture);
        return string.IsNullOrWhiteSpace(localizedString) ? key : localizedString;
    }

    public IRoom GetRoom(string roomId)
    {
        return _roomsManager.GetRoom(roomId);
    }

    public string GetRoomParameterValue(string roomId, string key)
    {
        return _roomsManager.GetRoomBotConfigurationParameterValue(roomId, key);
    }
}