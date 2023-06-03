using RazorLight;

namespace ElsaMina.Core.Services.Templating;

public class TemplatesManager : ITemplatesManager
{
    private const string TEMPLATES_DIRECTORY = "Templates";

    private readonly Lazy<RazorLightEngine> _razorLightEngine = new(() => new RazorLightEngineBuilder()
        .UseFileSystemProject(Path.Join(Environment.CurrentDirectory, TEMPLATES_DIRECTORY))
        .UseMemoryCachingProvider()
        .Build());

    public async Task<string> GetTemplate(string templateName, object model)
    {
        return await _razorLightEngine.Value.CompileRenderAsync(templateName, model);
    }
}