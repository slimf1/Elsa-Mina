using System.Diagnostics;

namespace ElsaMina.Core.Services.Telemetry;

public interface ITelemetryService
{
    Activity StartActivity(string name);

    void RecordMessageReceived(string room, string type);
    void RecordMessageSent();
    void RecordCommandExecuted(string commandName, string status);
    void RecordCommandError(string commandName);
    void RecordCommandDuration(double milliseconds, string commandName);
    void RecordWebSocketReconnection();
    void RecordHttpRequest(string method, string host, int status);
    void RecordHttpRequestDuration(double milliseconds, string method, string host);
}
