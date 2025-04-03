using System.Globalization;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Contexts;

public class DefaultContextProvider : IContextProvider
{
    private readonly IConfiguration _configuration;
    private readonly IResourcesService _resourcesService;
    private readonly IRoomsManager _roomsManager;

    public DefaultContextProvider(IConfiguration configuration,
        IResourcesService resourcesService,
        IRoomsManager roomsManager)
    {
        _configuration = configuration;
        _resourcesService = resourcesService;
        _roomsManager = roomsManager;
    }

    public string DefaultRoom => _configuration.DefaultRoom;
    public CultureInfo DefaultCulture => new(_configuration.DefaultLocaleCode);

    public bool IsUserWhitelisted(string userId)
    {
        return _configuration.Whitelist.Contains(userId);
    }

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