using System.Globalization;
using DotLiquid;
using DotLiquid.FileSystems;
using ElsaMina.Core.Services.Files;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Services.Templating;

public class TemplatesManager : ITemplatesManager
{
    private const string TEMPLATES_DIRECTORY = "Templates";

    static TemplatesManager()
    {
        Template.FileSystem = new LocalFileSystem(Path.Join(Environment.CurrentDirectory, TEMPLATES_DIRECTORY));
        Template.RegisterFilter(typeof(LocalizationFilter));
        Template.RegisterSafeType(typeof(CultureInfo), new[] { "*" });
        Template.RegisterSafeType(typeof(RoomParameters), new[] { "*" });
    }

    private static readonly Template CouldNotFoundFileTemplate = Template.Parse("""
        <h1 style="color: red;">Error</h1>
        Could not find template. Path = {{ path }}
    """);

    private readonly IFilesService _filesService;
    private readonly Dictionary<string, Template> _templatesCache = new();

    public TemplatesManager(IFilesService filesService)
    {
        _filesService = filesService;
    }

    public async Task<string> GetTemplate(string templateName, IDictionary<string, object> arguments)
    {
        if (!templateName.EndsWith(".liquid"))
        {
            templateName += ".liquid";
        }

        var templateFilePath = Path.Join(TEMPLATES_DIRECTORY, templateName);
        if (_templatesCache.TryGetValue(templateFilePath, out var value))
        {
            return value.Render(Hash.FromDictionary(arguments));
        }

        if (!_filesService.FileExists(templateFilePath))
        {
            return CouldNotFoundFileTemplate.Render(new Hash
            {
                ["path"] = templateFilePath
            });
        }

        var templateContent = await _filesService.ReadTextAsync(templateFilePath);
        var template = Template.Parse(templateContent);
        _templatesCache[templateFilePath] = template;

        return template.Render(Hash.FromDictionary(arguments));
    }
}