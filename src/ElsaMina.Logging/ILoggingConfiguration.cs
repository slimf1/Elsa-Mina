namespace ElsaMina.Logging;

public interface ILoggingConfiguration
{
    string LokiUrl { get; set; }
    string LoginUser { get; set; }
    string LokiApiKey { get; set; }
}