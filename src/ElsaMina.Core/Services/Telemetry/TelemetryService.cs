using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ElsaMina.Core.Services.Telemetry;

public class TelemetryService : ITelemetryService
{
    public const string SERVICE_NAME = "elsa-mina";
    public const string ACTIVITY_SOURCE_NAME = "ElsaMina";
    public const string METER_NAME = "ElsaMina";

    private readonly ActivitySource _activitySource = new(ACTIVITY_SOURCE_NAME);
    private readonly Meter _meter = new(METER_NAME);

    private readonly Counter<long> _messagesReceived;
    private readonly Counter<long> _messagesSent;
    private readonly Counter<long> _commandsExecuted;
    private readonly Counter<long> _commandErrors;
    private readonly Histogram<double> _commandDuration;
    private readonly Counter<long> _webSocketReconnections;
    private readonly Counter<long> _httpRequestsTotal;
    private readonly Histogram<double> _httpRequestDuration;

    public TelemetryService()
    {
        _messagesReceived = _meter.CreateCounter<long>("messages.received",
            description: "Total messages received from server");
        _messagesSent = _meter.CreateCounter<long>("messages.sent",
            description: "Total messages sent to server");
        _commandsExecuted = _meter.CreateCounter<long>("commands.executed",
            description: "Total commands executed");
        _commandErrors = _meter.CreateCounter<long>("commands.errors",
            description: "Total command execution errors");
        _commandDuration = _meter.CreateHistogram<double>("commands.duration_ms", unit: "ms",
            description: "Command execution duration in milliseconds");
        _webSocketReconnections = _meter.CreateCounter<long>("websocket.reconnections",
            description: "Total WebSocket reconnection events");
        _httpRequestsTotal = _meter.CreateCounter<long>("http.client.requests",
            description: "Total outbound HTTP requests");
        _httpRequestDuration = _meter.CreateHistogram<double>("http.client.request.duration_ms", unit: "ms",
            description: "Outbound HTTP request duration in milliseconds");
    }

    public Activity StartActivity(string name) => _activitySource.StartActivity(name);

    public void RecordMessageReceived(string room, string type) =>
        _messagesReceived.Add(1,
            new KeyValuePair<string, object>("room", room),
            new KeyValuePair<string, object>("type", type));

    public void RecordMessageSent() => _messagesSent.Add(1);

    public void RecordCommandExecuted(string commandName, string status) =>
        _commandsExecuted.Add(1,
            new KeyValuePair<string, object>("command", commandName),
            new KeyValuePair<string, object>("status", status));

    public void RecordCommandError(string commandName) =>
        _commandErrors.Add(1,
            new KeyValuePair<string, object>("command", commandName));

    public void RecordCommandDuration(double milliseconds, string commandName) =>
        _commandDuration.Record(milliseconds,
            new KeyValuePair<string, object>("command", commandName));

    public void RecordWebSocketReconnection() => _webSocketReconnections.Add(1);

    public void RecordHttpRequest(string method, string host, int status) =>
        _httpRequestsTotal.Add(1,
            new KeyValuePair<string, object>("method", method),
            new KeyValuePair<string, object>("host", host),
            new KeyValuePair<string, object>("status", status));

    public void RecordHttpRequestDuration(double milliseconds, string method, string host) =>
        _httpRequestDuration.Record(milliseconds,
            new KeyValuePair<string, object>("method", method),
            new KeyValuePair<string, object>("host", host));
}
