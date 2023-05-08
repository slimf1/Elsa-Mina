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

    private IEnumerable<CultureInfo> _supportedLocales;

    public ResourcesService(IConfigurationManager configurationManager)
    {
        _defaultLocale = new CultureInfo(configurationManager.Configuration.DefaultLocaleCode);
    }
    
    public IEnumerable<CultureInfo> SupportedLocales => _supportedLocales ??= GetSupportedLocales();

    public string GetString(string key, CultureInfo cultureInfo = null)
    {
        return _resourceManager.Value.GetString(key, cultureInfo ?? _defaultLocale);
    }
    
    private IEnumerable<CultureInfo> GetSupportedLocales()
    {
        var supportedLocales = new List<CultureInfo>();
        foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
        {
            try
            {
                var resourceSet = _resourceManager.Value.GetResourceSet(cultureInfo, true, false);
                if (resourceSet == null)
                {
                    continue;
                }
                supportedLocales.Add(cultureInfo);
            }
            catch (CultureNotFoundException)
            {
                // Do nothing
            }
        }
        return supportedLocales;
    }
}