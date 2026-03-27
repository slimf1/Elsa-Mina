using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ElsaMina.Core;

public static class Telemetry
{
    public const string SERVICE_NAME = "elsa-mina";
    public const string ACTIVITY_SOURCE_NAME = "ElsaMina";
    public const string METER_NAME = "ElsaMina";

    public static readonly ActivitySource ACTIVITY_SOURCE = new(ACTIVITY_SOURCE_NAME);
    public static readonly Meter METER = new(METER_NAME);

    public static readonly Counter<long> MESSAGES_RECEIVED =
        METER.CreateCounter<long>("messages.received", description: "Total messages received from server");

    public static readonly Counter<long> MESSAGES_SENT =
        METER.CreateCounter<long>("messages.sent", description: "Total messages sent to server");

    public static readonly Counter<long> COMMANDS_EXECUTED =
        METER.CreateCounter<long>("commands.executed", description: "Total commands executed");

    public static readonly Counter<long> COMMAND_ERRORS =
        METER.CreateCounter<long>("commands.errors", description: "Total command execution errors");

    public static readonly Histogram<double> COMMAND_DURATION =
        METER.CreateHistogram<double>("commands.duration_ms", unit: "ms",
            description: "Command execution duration in milliseconds");

    public static readonly Counter<long> WEB_SOCKET_RECONNECTIONS =
        METER.CreateCounter<long>("websocket.reconnections", description: "Total WebSocket reconnection events");

    public static readonly Counter<long> HTTP_REQUESTS_TOTAL =
        METER.CreateCounter<long>("http.client.requests", description: "Total outbound HTTP requests");

    public static readonly Histogram<double> HTTP_REQUEST_DURATION =
        METER.CreateHistogram<double>("http.client.request.duration_ms", unit: "ms",
            description: "Outbound HTTP request duration in milliseconds");
}
