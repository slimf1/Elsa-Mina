using System.Globalization;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;

namespace ElsaMina.Core.Contexts;

public class DefaultContextProvider : IContextProvider
{
    private readonly IConfigurationManager _configurationManager;
    private readonly IResourcesService _resourcesService;

    public DefaultContextProvider(IConfigurationManager configurationManager, IResourcesService resourcesService)
    {
        _configurationManager = configurationManager;
        _resourcesService = resourcesService;
    }

    public IEnumerable<string> CurrentWhitelist => _configurationManager.Configuration.Whitelist;
    public string DefaultRoom => _configurationManager.Configuration.DefaultRoom;
    public CultureInfo DefaultCulture => new(_configurationManager.Configuration.DefaultLocaleCode);

    public string GetString(string key, CultureInfo culture)
    {
        var localizedString = _resourcesService.GetString(key, culture ?? DefaultCulture);
        return string.IsNullOrWhiteSpace(localizedString) ? key : localizedString;
    }
}