using Serilog;

namespace ElsaMina.Core;

public static class Log
{
    private static readonly Serilog.Core.Logger Logger = GetLogger();

    private static Serilog.Core.Logger GetLogger()
    {
        var loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .WriteTo.Console();

#if !DEBUG
            loggerConfig.MinimumLevel.Information();
            loggerConfig.WriteTo.File("log.txt", rollingInterval: RollingInterval.Day);
#endif
        return loggerConfig.CreateLogger();
    }

    public static void Information(string message) => Logger.Information(message);
    public static void Information(string messageTemplate, params object[] propertyValues) => Logger.Information(messageTemplate, propertyValues);
    public static void Error(string message) => Logger.Error(message);
    public static void Error(string messageTemplate, params object[] propertyValues) => Logger.Error(messageTemplate, propertyValues);
    public static void Error(Exception exception, string messageTemplate, params object[] propertyValues) => Logger.Error(exception, messageTemplate, propertyValues);
    public static void Debug(string message) => Logger.Debug(message);
    public static void Debug(string messageTemplate, params object[] propertyValues) => Logger.Debug(messageTemplate, propertyValues);
    public static void Warning(string message) => Logger.Warning(message);
    public static void Warning(string messageTemplate, params object[] propertyValues) => Logger.Warning(messageTemplate, propertyValues);

}
