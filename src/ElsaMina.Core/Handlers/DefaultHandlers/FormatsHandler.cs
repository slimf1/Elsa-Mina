using ElsaMina.Core.Services.Formats;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public class FormatsHandler : Handler
{
    private readonly IFormatsManager _formatsManager;

    public FormatsHandler(IFormatsManager formatsManager)
    {
        _formatsManager = formatsManager;
    }

    public override Task HandleReceivedMessageAsync(string[] parts, string roomId = null, CancellationToken cancellationToken = default)
    {
        if (parts.Length >= 5 && parts[1] == "formats")
        {
            _formatsManager.ParseFormats(parts[4..]);
        }

        return Task.CompletedTask;
    }
}