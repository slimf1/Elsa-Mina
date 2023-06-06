using ElsaMina.Core.Templates;

namespace ElsaMina.Core.Services.Templating;

public interface ITemplatesManager
{
    Task<string> GetTemplate(string templateName, object model);
    Task<string> GetTemplate<TPage, TViewModel>()
        where TPage : LocalizableTemplatePage<LocalizableViewModel>
        where TViewModel : LocalizableViewModel;
}