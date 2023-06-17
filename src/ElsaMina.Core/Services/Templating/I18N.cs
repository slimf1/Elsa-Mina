using System.Globalization;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Resources;

namespace ElsaMina.Core.Services.Templating;

public static class I18N
{
    public static string GetString(CultureInfo culture, string key, params object[] formatArguments)
    {
        return string.Format(DependencyContainerService.s_ContainerService.Resolve<IResourcesService>()
            .GetString(key, culture), formatArguments);
    }
}