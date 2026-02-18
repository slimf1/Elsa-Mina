using Autofac;
using ElsaMina.Core.Services.Config;
using ElsaMina.DataAccess;
using ElsaMina.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ElsaMina.Core.Modules;

public class DataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.Register(GetDbContextOptions)
            .As<DbContextOptions<BotDbContext>>()
            .SingleInstance();

        builder.RegisterType<BotDbContext>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.Register(ctx =>
            {
                var options = ctx.Resolve<DbContextOptions<BotDbContext>>();
                return new PooledDbContextFactory<BotDbContext>(options);
            })
            .As<IDbContextFactory<BotDbContext>>()
            .SingleInstance();

        builder.RegisterType<BotDbContextFactory>().As<IBotDbContextFactory>().SingleInstance();
    }

    private static DbContextOptions<BotDbContext> GetDbContextOptions(IComponentContext ctx)
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
            .LogTo(message => Log.Debug(message), LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
#endif
        return optionsBuilder.Options;
    }
}