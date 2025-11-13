using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Misc.Genius;

[NamedCommand("genius", "lyrics", "song")]
public class GeniusSearchCommand : Command
{
    private readonly IConfiguration _configuration;
    private readonly IHttpService _httpService;

    public GeniusSearchCommand(IConfiguration configuration, IHttpService httpService)
    {
        _configuration = configuration;
        _httpService = httpService;
    }

    public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}