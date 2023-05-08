using System.Globalization;
using System.Reflection;
using System.Resources;

namespace ElsaMina.Core.Services.Resources;

public class ResourcesService : IResourcesService
{
    private readonly Lazy<ResourceManager> _resourceManager =
        new(() => new ResourceManager("ElsaMina.Core.Resources.Resources", Assembly.GetExecutingAssembly()));

    public string GetString(string key, CultureInfo cultureInfo = null)
    {
        return _resourceManager.Value.GetString(key, cultureInfo ?? CultureInfo.CurrentCulture);
    }
}