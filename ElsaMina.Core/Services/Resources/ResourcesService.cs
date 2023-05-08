using System.Globalization;
using System.Reflection;
using System.Resources;
using ElsaMina.Core.Services.Config;

namespace ElsaMina.Core.Services.Resources;

public class ResourcesService : IResourcesService
{
    private readonly CultureInfo _defaultLocale;
    private readonly Lazy<ResourceManager> _resourceManager =
        new(() => new ResourceManager("ElsaMina.Core.Resources.Resources", Assembly.GetExecutingAssembly()));

    public ResourcesService(IConfigurationManager configurationManager)
    {
        _defaultLocale = new CultureInfo(configurationManager.Configuration.DefaultLocaleCode);
    }

    public string GetString(string key, CultureInfo cultureInfo = null)
    {
        return _resourceManager.Value.GetString(key, cultureInfo ?? _defaultLocale);
    }
}