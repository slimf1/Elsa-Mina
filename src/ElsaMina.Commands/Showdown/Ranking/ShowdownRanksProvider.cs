using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Showdown.Ranking;

public class ShowdownRanksProvider : IShowdownRanksProvider
{
    private const string RANK_RESOURCE_URL =
        "https://play.pokemonshowdown.com/~~showdown/action.php?act=ladderget&user={0}";

    private readonly IHttpService _httpService;

    public ShowdownRanksProvider(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<IEnumerable<RankingDataDto>> GetRankingDataAsync(string userId,
        CancellationToken cancellationToken = default)
    {
        var result = await _httpService.GetAsync<IEnumerable<RankingDataDto>>(
            string.Format(RANK_RESOURCE_URL, userId),
            removeFirstCharacterFromResponse: true,
            cancellationToken: cancellationToken);
        return result.Data;
    }
}