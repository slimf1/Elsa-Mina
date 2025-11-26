using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ElsaMina.DataAccess;

/// <summary>
/// Needed to let EF Core know that I'm using PGSQL
/// </summary>
[UsedImplicitly]
public class BotDbContextDesignTimeFactory : IDesignTimeDbContextFactory<BotDbContext>
{
    public BotDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BotDbContext>();
        optionsBuilder.UseNpgsql();
        return new BotDbContext(optionsBuilder.Options);
    }
}