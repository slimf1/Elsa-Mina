using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace ElsaMina.Logging;

public class ShowdownSink : ILogEventSink
{
    private const int MAX_MESSAGE_LENGTH = 300;

    private static readonly MessageTemplateTextFormatter FORMATTER =
        new("{Level:u3}: {Message:lj}{NewLine}{Exception}");

    public static Action<string, string>? BotSender { get; set; }
    public static string? RoomId { get; set; }

    public void Emit(LogEvent logEvent)
    {
        var sender = BotSender;
        var roomId = RoomId;
        if (sender == null || string.IsNullOrEmpty(roomId))
        {
            return;
        }

        using var writer = new StringWriter();
        FORMATTER.Format(logEvent, writer);
        var message = writer.ToString().Trim();

        if (message.Length > MAX_MESSAGE_LENGTH)
        {
            message = string.Concat(message.AsSpan(0, MAX_MESSAGE_LENGTH), "...");
        }

        sender(roomId, message);
    }
}
