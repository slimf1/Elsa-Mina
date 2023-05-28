using System.Globalization;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Services.Resources;

namespace ElsaMina.Core.Services.Templating;

public class LocalizationFilter
{
    private static IResourcesService _resourcesService;
    
    public static string Localize(string input, string cultureName, string arg0 = null, string arg1 = null, string arg2 = null, string arg3 = null)
    {
        _resourcesService ??= DependencyContainerService.s_ContainerService.Resolve<IResourcesService>();
        var culture = new CultureInfo(cultureName);
        return string.Format(_resourcesService.GetString(input, culture), arg0, arg1, arg2, arg3);
    }
}