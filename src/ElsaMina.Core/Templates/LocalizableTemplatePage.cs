using ElsaMina.Core.Services.Templating;
using RazorLight;

namespace ElsaMina.Core.Templates;

public abstract class LocalizableTemplatePage<TViewModel> : TemplatePage<TViewModel>
    where TViewModel : LocalizableViewModel
{
    protected string GetString(string key, params object[] formatArguments)
    {
        return I18N.GetString(Model.Culture, key, formatArguments);
    }
}