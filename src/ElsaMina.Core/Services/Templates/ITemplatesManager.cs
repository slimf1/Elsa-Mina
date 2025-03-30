namespace ElsaMina.Core.Services.Templates;

public interface ITemplatesManager
{
    Task CompileTemplates(); // TODO : à utiliser pour un hot-reload des templates, mais copié au build time
    Task<string> GetTemplateAsync(string templateKey, object model);
}
