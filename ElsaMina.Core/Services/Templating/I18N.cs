using System.Globalization;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Resources;

namespace ElsaMina.Core.Services.Templating;

public static class I18N
{
    public static string S(string cultureName, string key, params object[] formatArguments)
    {
        return string.Format(DependencyContainerService.s_ContainerService.Resolve<IResourcesService>()
            .GetString(key, new CultureInfo(cultureName)), formatArguments);
    }
}