namespace ElsaMina.Core.Services.Templating;

public interface ITemplatesManager
{
    Task<string> GetTemplate(string templateName, IDictionary<string, object> arguments);
}