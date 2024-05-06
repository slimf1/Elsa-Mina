using ElsaMina.Core.Services.DependencyInjection;

namespace ElsaMina.Core.Parsers;

public class ParsersManager : IParsersManager
{
    private readonly IDependencyContainerService _containerService;
    
    private IEnumerable<IParser> _parsers = Enumerable.Empty<IParser>();

    public ParsersManager(IDependencyContainerService containerService)
    {
        _containerService = containerService;
    }
    
    public bool IsInitialized { get; private set; }

    public async Task Initialize()
    {
        _parsers = _containerService.Resolve<IEnumerable<IParser>>();
        await Task.WhenAll(
            _parsers.Select(parser => parser.OnInitialize())
        );
        IsInitialized = true;
    }

    public async Task Parse(string[] parts, string roomId = null)
    {
        await Task.WhenAll(
            _parsers
                .Where(parser => parser.IsEnabled)
                .Select(parser => parser.Invoke(parts, roomId))
        );
    }
}