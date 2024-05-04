using ElsaMina.Core.Templates;

namespace ElsaMina.Core.Services.Templates;

public interface ITemplatesManager
{
    Task PreCompileTemplates();
    Task<string> GetTemplate(string templateKey, object model);
    Task<string> GetTemplate<TPage, TViewModel>()
        where TPage : LocalizableTemplatePage<LocalizableViewModel>
        where TViewModel : LocalizableViewModel;
}