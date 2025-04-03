using System.Collections.Concurrent;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Templates;
using ElsaMina.Core.Utils;
using RazorLight;

namespace ElsaMina.Core.Services.Templates;

public class TemplatesManager : ITemplatesManager
{
    private const string TEMPLATES_DIRECTORY_NAME = "Templates";

    private static readonly string TEMPLATES_DIRECTORY_PATH =
        Path.Join(Environment.CurrentDirectory, TEMPLATES_DIRECTORY_NAME);

    private static readonly RazorLightEngine RAZOR_ENGINE = new RazorLightEngineBuilder()
        .UseFileSystemProject(TEMPLATES_DIRECTORY_PATH)
        .UseMemoryCachingProvider()
        .Build();

    private readonly ConcurrentDictionary<string, ITemplatePage> _compilationResults = new();

    public async Task<string> GetTemplateAsync(string templateKey, object model)
    {
        if (!_compilationResults.TryGetValue(templateKey, out var compiledTemplatePage))
        {
            return null;
        }

        var template = await RAZOR_ENGINE.RenderTemplateAsync(compiledTemplatePage, model);
        return template.RemoveNewlines().RemoveWhitespacesBetweenTags();
    }

    public async Task CompileTemplatesAsync()
    {
        var compilationTasks = FileSystem
            .GetFilesFromDirectoryRecursively(TEMPLATES_DIRECTORY_PATH)
            .Select(CompileTemplateAsync);
        await Task.WhenAll(compilationTasks);
    }

    private async Task CompileTemplateAsync(string templatePath)
    {
        Log.Information("Compiling template {0}...", templatePath);
        var templateKey = GetTemplateKeyFromPath(templatePath);
        _compilationResults[templateKey] = await RAZOR_ENGINE.CompileTemplateAsync(templatePath);
    }

    private static string GetTemplateKeyFromPath(string templatePath)
    {
        return FileSystem
            .MakeRelativePath(templatePath, TEMPLATES_DIRECTORY_PATH)
            .Substring(TEMPLATES_DIRECTORY_NAME.Length + 1)
            .RemoveExtension();
    }
}