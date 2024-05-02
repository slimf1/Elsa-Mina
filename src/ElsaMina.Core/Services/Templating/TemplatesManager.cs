using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Templates;
using ElsaMina.Core.Utils;
using RazorLight;
using Serilog;

namespace ElsaMina.Core.Services.Templating;

public class TemplatesManager : ITemplatesManager
{
    private const string TEMPLATES_DIRECTORY = "Templates";

    private static readonly string TEMPLATES_DIRECTORY_PATH =
        Path.Join(Environment.CurrentDirectory, TEMPLATES_DIRECTORY);

    private static readonly RazorLightEngine RAZOR_ENGINE = new RazorLightEngineBuilder()
        .UseFileSystemProject(TEMPLATES_DIRECTORY_PATH)
        .UseMemoryCachingProvider()
        .Build();

    private readonly IDependencyContainerService _dependencyContainerService;
    private readonly ILogger _logger;

    public TemplatesManager(IDependencyContainerService dependencyContainerService, ILogger logger)
    {
        _dependencyContainerService = dependencyContainerService;
        _logger = logger;
    }

    public async Task<string> GetTemplate(string templateName, object model)
    {
        return await RAZOR_ENGINE.CompileRenderAsync(templateName, model);
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

    private async Task PreCompileTemplate(string templateKey)
    {
        _logger.Information("Pre-compiling template {0}", templateKey);
        await RAZOR_ENGINE.CompileTemplateAsync(templateKey);
        _logger.Information("Pre-compiling template {0}: DONE", templateKey);
    }
}