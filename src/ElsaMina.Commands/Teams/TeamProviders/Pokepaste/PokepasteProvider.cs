using ElsaMina.Core;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Teams.TeamProviders.Pokepaste;

public class PokepasteProvider : ITeamProvider
{
    private readonly IHttpService _httpService;

    public PokepasteProvider(IHttpService httpService)
    {
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
            Logger.Current.Error(exception, "An error occurred while fetching a postepaste team");
            return null;
        }
    }
}