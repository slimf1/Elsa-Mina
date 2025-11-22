using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ElsaMina.DataAccess;

[UsedImplicitly]
public class BotDbContextDesignTimeFactory : IDesignTimeDbContextFactory<BotDbContext>
{
    public BotDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BotDbContext>();
        
        // Use a hardcoded or environment-based connection string for migrations
        optionsBuilder.UseNpgsql();
        
        return new BotDbContext(optionsBuilder.Options);
    }
}