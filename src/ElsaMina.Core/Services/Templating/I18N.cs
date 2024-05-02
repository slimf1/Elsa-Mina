using System.Globalization;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Resources;

namespace ElsaMina.Core.Services.Templating;

public static class I18N
{
    private static Lazy<IResourcesService> ResourcesService => new(
        () => DependencyContainerService.Current.Resolve<IResourcesService>()
    );

    public static string GetString(CultureInfo culture, string key, params object[] formatArguments)
    {
        return string.Format(ResourcesService.Value.GetString(key, culture), formatArguments);
    }
}