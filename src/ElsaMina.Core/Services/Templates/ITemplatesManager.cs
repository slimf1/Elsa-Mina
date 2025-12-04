namespace ElsaMina.Core.Services.Templates;

public interface ITemplatesManager
{
    Task CompileTemplatesAsync();
    Task<string> GetTemplateAsync(string templateKey, object model);
}
