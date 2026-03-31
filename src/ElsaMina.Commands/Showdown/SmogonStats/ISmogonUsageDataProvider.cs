namespace ElsaMina.Commands.Showdown.SmogonStats;

public interface ISmogonUsageDataProvider
{
    Task<SmogonUsageDataDto> GetUsageDataAsync(string month, string format, int playerLevel,
        CancellationToken cancellationToken = default);
}
