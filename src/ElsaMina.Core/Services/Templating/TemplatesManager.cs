using ElsaMina.Core.Services.DependencyInjection;
using ElsaMina.Core.Templates;
using RazorLight;

namespace ElsaMina.Core.Services.Templating;

public class TemplatesManager : ITemplatesManager
{
    private const string TEMPLATES_DIRECTORY = "Templates";

    private readonly Lazy<RazorLightEngine> _razorLightEngine = new(() => new RazorLightEngineBuilder()
        .UseFileSystemProject(Path.Join(Environment.CurrentDirectory, TEMPLATES_DIRECTORY))
        .UseMemoryCachingProvider()
        .Build());

    private readonly IDependencyContainerService _dependencyContainerService;

    public TemplatesManager(IDependencyContainerService dependencyContainerService)
    {
        _dependencyContainerService = dependencyContainerService;
    }

    public async Task<string> GetTemplate(string templateName, object model)
    {
        return await _razorLightEngine.Value.CompileRenderAsync(templateName, model);
    }

    public async Task<string> GetTemplate<TPage, TViewModel>()
        where TPage : LocalizableTemplatePage<LocalizableViewModel>
        where TViewModel : LocalizableViewModel
    {
        var template = _dependencyContainerService.Resolve<TPage>();
        var viewModel = _dependencyContainerService.Resolve<TViewModel>();
        return await _razorLightEngine.Value.RenderTemplateAsync(template, viewModel);
    }
}