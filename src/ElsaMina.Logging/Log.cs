using Serilog;
using Serilog.Core;
using Serilog.Sinks.Grafana.Loki;

namespace ElsaMina.Logging;

public static class Log
{
    private static readonly Lazy<Logger> LOGGER = new(CreateLogger, isThreadSafe: true);

    private static ILogger Instance => LOGGER.Value;
    
    public static ILoggingConfiguration Configuration { get; set; }

    private static Logger CreateLogger()
    {
        var config = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .WriteTo.Console();

#if !DEBUG
        config.MinimumLevel.Information()
              .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day);

        var lokiUrl = Configuration.LokiUrl;
        var lokiUser = Configuration.LoginUser;
        var lokiApiKey = Configuration.LokiApiKey;

        if (!string.IsNullOrWhiteSpace(lokiUrl) &&
            !string.IsNullOrWhiteSpace(lokiUser) &&
            !string.IsNullOrWhiteSpace(lokiApiKey))
        {
            config.WriteTo.GrafanaLoki(
                lokiUrl,
                credentials: new LokiCredentials
                {
                    Login = lokiUser,
                    Password = lokiApiKey
                },
                labels:
                [
                    new LokiLabel { Key = "app", Value = "elsamina-core" },
                    new LokiLabel { Key = "env", Value = "prod" },
                    new LokiLabel { Key = "host", Value = Environment.MachineName }
                ]);
        }
#endif

        return config.CreateLogger();
    }

    public static void Information(string messageTemplate, params object[] propertyValues) =>
        Instance.Information(messageTemplate, propertyValues);

    public static void Warning(string messageTemplate, params object[] propertyValues) =>
        Instance.Warning(messageTemplate, propertyValues);

    public static void Error(string messageTemplate, params object[] propertyValues) =>
        Instance.Error(messageTemplate, propertyValues);

    public static void Error(Exception ex, string messageTemplate, params object[] propertyValues) =>
        Instance.Error(ex, messageTemplate, propertyValues);

    public static void Debug(string messageTemplate, params object[] propertyValues) =>
        Instance.Debug(messageTemplate, propertyValues);

    public static void CloseAndFlush() => Serilog.Log.CloseAndFlush();
}
