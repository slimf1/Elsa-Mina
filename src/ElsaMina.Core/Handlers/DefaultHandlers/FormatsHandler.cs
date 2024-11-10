using ElsaMina.Core.Services.Formats;

namespace ElsaMina.Core.Handlers.DefaultHandlers;

public class FormatsHandler : Handler
{
    private readonly IFormatsManager _formatsManager;

    public FormatsHandler(IFormatsManager formatsManager)
    {
        _formatsManager = formatsManager;
    }

    public override string Identifier => nameof(FormatsHandler);

    public override Task HandleReceivedMessage(string[] parts, string roomId = null)
    {
        if (parts.Length >= 1 && parts[0] == "format")
        {
            _formatsManager.ParseFormats(parts[4..]);
        }

        return Task.CompletedTask;
    }
}