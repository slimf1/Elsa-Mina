using Autofac;
using ElsaMina.Core.Services.Config;
using ElsaMina.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElsaMina.Core.Modules;

public class DataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder
            .Register(ctx =>
            {
                var config = ctx.Resolve<IConfiguration>();
                var optionsBuilder = new DbContextOptionsBuilder<BotDbContext>();
                optionsBuilder.UseNpgsql(
                    config.ConnectionString,
                    npgsql => npgsql.EnableRetryOnFailure(
                        maxRetryCount: config.DatabaseMaxRetries,
                        maxRetryDelay: config.DatabaseRetryDelay,
                        errorCodesToAdd: null)
                );
#if DEBUG
                optionsBuilder
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
#endif
                return new BotDbContext(optionsBuilder.Options);
            })
            .AsSelf()
            .InstancePerLifetimeScope();
    }
}