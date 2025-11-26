namespace ElsaMina.DataAccess;

public interface IBotDbContextFactory
{
    Task<BotDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default);
}