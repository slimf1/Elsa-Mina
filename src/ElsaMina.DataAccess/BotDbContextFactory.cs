using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess;

public class BotDbContextFactory : IBotDbContextFactory
{
    private readonly IDbContextFactory<BotDbContext> _factory;

    public BotDbContextFactory(IDbContextFactory<BotDbContext> factory)
    {
        _factory = factory;
    }

    public Task<BotDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return _factory.CreateDbContextAsync(cancellationToken);
    }
}