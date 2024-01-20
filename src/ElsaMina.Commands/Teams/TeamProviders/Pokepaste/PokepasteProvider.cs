using ElsaMina.Core.Services.Http;
using Serilog;

namespace ElsaMina.Commands.Teams.TeamProviders.Pokepaste;

public class PokepasteProvider : ITeamProvider
{
    private readonly ILogger _logger;
    private readonly IHttpService _httpService;

    public PokepasteProvider(ILogger logger, IHttpService httpService)
    {
        _logger = logger;
        _httpService = httpService;
    }

    public async Task<SharedTeam> GetTeamExport(string teamLink)
    {
        try
        {
            var pokepasteTeam = await _httpService.Get<PokepasteTeam>(teamLink.Trim() + "/json");
            return new SharedTeam
            {
                Title = pokepasteTeam.Title,
                Description = pokepasteTeam.Notes,
                Author = pokepasteTeam.Author,
                TeamExport = pokepasteTeam.Paste
            };
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "An error occurred while fetching a postepaste team");
            return null;
        }
    }
}