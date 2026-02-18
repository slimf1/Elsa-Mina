namespace ElsaMina.Logging;

public interface ILoggingConfiguration
{
    LogLevel LogLevel { get; }
    string LokiUrl { get; set; }
    string LoginUser { get; set; }
    string LokiApiKey { get; set; }
}