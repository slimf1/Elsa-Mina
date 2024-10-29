using ElsaMina.Core.Constants;
using Serilog;

namespace ElsaMina.Core;

public static class Logger
{
    private static readonly Serilog.Core.Logger LOGGER;

    static Logger()
    {
        var loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .WriteTo.Console();

        if (!EnvironmentConstants.IS_DEBUG)
        {
            // Built in release
            loggerConfig.MinimumLevel.Information();
            loggerConfig.WriteTo.File("log.txt", rollingInterval: RollingInterval.Day);
        }

        LOGGER = loggerConfig.CreateLogger();
    }

    public static void Information(string message) => LOGGER.Information(message);
    public static void Information(string messageTemplate, params object[] propertyValues) => LOGGER.Information(messageTemplate, propertyValues);
    public static void Error(string message) => LOGGER.Error(message);
    public static void Error(string messageTemplate, params object[] propertyValues) => LOGGER.Error(messageTemplate, propertyValues);
    public static void Error(Exception exception, string messageTemplate, params object[] propertyValues) => LOGGER.Error(exception, messageTemplate, propertyValues);
    public static void Debug(string message) => LOGGER.Debug(message);
    public static void Debug(string messageTemplate, params object[] propertyValues) => LOGGER.Debug(messageTemplate, propertyValues);
    public static void Warning(string message) => LOGGER.Warning(message);
    public static void Warning(string messageTemplate, params object[] propertyValues) => LOGGER.Warning(messageTemplate, propertyValues);

}
