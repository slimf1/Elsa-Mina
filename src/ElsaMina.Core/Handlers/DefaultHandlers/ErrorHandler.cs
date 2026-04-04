using ElsaMina.Logging;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public class ErrorHandler : Handler
{
    public override IReadOnlySet<string> HandledMessageTypes => new HashSet<string> { "error" };

    public override Task HandleReceivedMessageAsync(string[] parts, string roomId = null,
        CancellationToken cancellationToken = default)
    {
        if (parts.Length <= 3 || parts[1] != "error")
        {
            return Task.CompletedTask;
        }

        Log.Error("Received error message from server : {Error}", parts[2]);
        return Task.CompletedTask;
    }
}