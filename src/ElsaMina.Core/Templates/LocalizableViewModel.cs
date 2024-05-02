using System.Globalization;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.DependencyInjection;

namespace ElsaMina.Core.Templates;

public class LocalizableViewModel
{
    protected LocalizableViewModel()
    {
        Culture = new CultureInfo(
            DependencyContainerService.Current.Resolve<IConfigurationManager>().Configuration.DefaultLocaleCode
        );
    }
    
    public CultureInfo Culture { get; set; }
}