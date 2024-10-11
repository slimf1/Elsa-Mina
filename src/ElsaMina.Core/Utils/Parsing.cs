namespace ElsaMina.Core.Utils;

public static class Parsing
{
    public static (string target, string command) ParseMessage(string message, string trigger)
    {
        var triggerLength = trigger.Length;
        if (message[..triggerLength] != trigger)
        {
            return (null, null);
        }

        var text = message[triggerLength..];
        var spaceIndex = text.IndexOf(' ');
        var command = spaceIndex > 0 ? text[..spaceIndex].ToLower() : text.Trim().ToLower();
        var target = spaceIndex > 0 ? text[(spaceIndex + 1)..] : string.Empty;
        return string.IsNullOrEmpty(command) ? (null, null) : (target, command);
    }
}