using System.Globalization;
using System.Reflection;
using System.Resources;
using ElsaMina.Core.Services.Config;

namespace ElsaMina.Core.Services.Resources;

public class ResourcesService : IResourcesService
{
    private readonly CultureInfo _defaultCulture;

    private readonly Lazy<ResourceManager> _resourceManager =
        new(() => new ResourceManager("ElsaMina.Core.Resources.Resources", Assembly.GetExecutingAssembly()));

    private IEnumerable<CultureInfo> _supportedCultures;

    public ResourcesService(IConfiguration configuration)
    {
        _defaultCulture = new CultureInfo(configuration.DefaultLocaleCode);
    }

    public IEnumerable<CultureInfo> SupportedLocales => _supportedCultures ??= GetSupportedCultures();

    public string GetString(string key, CultureInfo cultureInfo = null)
    {
        try
        {
            return _resourceManager.Value.GetString(key, cultureInfo ?? _defaultCulture) ?? key;
        }
        catch (MissingManifestResourceException)
        {
            // If the resource is not found, return the key itself
            return key;
        }
    }

    private List<CultureInfo> GetSupportedCultures()
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