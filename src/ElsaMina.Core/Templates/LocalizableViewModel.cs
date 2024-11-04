using System.Globalization;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;

namespace ElsaMina.Core.Templates;

public class LocalizableViewModel
{
    private const string DEFAULT_CULTURE_CODE = "en-US";
    
    protected LocalizableViewModel()
    {
        Culture = new CultureInfo(
            DependencyContainerService.Current?.Resolve<IConfigurationManager>()?.Configuration?.DefaultLocaleCode
            ?? DEFAULT_CULTURE_CODE
        );
    }
    
    public CultureInfo Culture { get; set; }
}