namespace ElsaMina.Commands.Showdown.Ranking;

public interface IShowdownRanksProvider
{
    Task<IEnumerable<RankingDataDto>> GetRankingDataAsync(string userId,
        CancellationToken cancellationToken = default);
}