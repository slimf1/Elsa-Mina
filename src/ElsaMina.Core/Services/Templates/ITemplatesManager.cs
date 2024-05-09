using ElsaMina.Core.Templates;

namespace ElsaMina.Core.Services.Templates;

public interface ITemplatesManager
{
    Task CompileTemplates(); // TODO : à utiliser pour un hot-reload des templates, mais copié au build time
    Task<string> GetTemplate(string templateKey, object model);
    Task<string> GetTemplate<TPage, TViewModel>()
        where TPage : LocalizableTemplatePage<LocalizableViewModel>
        where TViewModel : LocalizableViewModel;
}