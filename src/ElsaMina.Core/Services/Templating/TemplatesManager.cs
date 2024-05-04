using System.Collections.Concurrent;
using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Templates;
using ElsaMina.Core.Utils;
using RazorLight;

namespace ElsaMina.Core.Services.Templating;

public class TemplatesManager : ITemplatesManager
{
    private const string TEMPLATES_DIRECTORY_NAME = "Templates";

    private static readonly string TEMPLATES_DIRECTORY_PATH =
        Path.Join(Environment.CurrentDirectory, TEMPLATES_DIRECTORY_NAME);

    private static readonly RazorLightEngine RAZOR_ENGINE = new RazorLightEngineBuilder()
        .UseFileSystemProject(TEMPLATES_DIRECTORY_PATH)
        .UseMemoryCachingProvider()
        .Build();

    private readonly IDependencyContainerService _dependencyContainerService;

    private readonly ConcurrentDictionary<string, ITemplatePage> _preCompilationResults = new();

    public TemplatesManager(IDependencyContainerService dependencyContainerService)
    {
        _dependencyContainerService = dependencyContainerService;
    }

    public async Task<string> GetTemplate(string templateKey, object model)
    {
        if (!_preCompilationResults.TryGetValue(templateKey, out var precompiledTemplatePage))
        {
            return null;
        }

        return await RAZOR_ENGINE.RenderTemplateAsync(precompiledTemplatePage, model);
    }

    public async Task PreCompileTemplates()
    {
        var compilationTasks = FileSystem
            .GetFilesFromDirectoryRecursively(TEMPLATES_DIRECTORY_PATH)
            .Select(PreCompileTemplate);
        await Task.WhenAll(compilationTasks);
    }

    public async Task<string> GetTemplate<TPage, TViewModel>()
        where TPage : LocalizableTemplatePage<LocalizableViewModel>
        where TViewModel : LocalizableViewModel
    {
        var template = _dependencyContainerService.Resolve<TPage>();
        var viewModel = _dependencyContainerService.Resolve<TViewModel>();
        return await RAZOR_ENGINE.RenderTemplateAsync(template, viewModel);
    }

    private async Task PreCompileTemplate(string templatePath)
    {
        Logger.Current.Information("Pre-compiling template {0}...", templatePath);
        var templateKey = GetTemplateKeyFromPath(templatePath);
        _preCompilationResults[templateKey] = await RAZOR_ENGINE.CompileTemplateAsync(templatePath);
    }

    private static string GetTemplateKeyFromPath(string templatePath)
    {
        return FileSystem
            .MakeRelativePath(templatePath, TEMPLATES_DIRECTORY_PATH)
            .Substring(TEMPLATES_DIRECTORY_NAME.Length + 1)
            .RemoveExtension();
    }
}