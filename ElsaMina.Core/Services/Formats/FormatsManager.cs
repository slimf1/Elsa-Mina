using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Services.Formats;

public class FormatsManager : IFormatsManager
{
    private readonly List<string> _formats = new();

    public IEnumerable<string> Formats => _formats;

    public void ParseFormatsFromReceivedLine(string message)
    {
        var formats = message.Split("|")[5..];
        foreach (var format in formats)
        {
            if (!format.StartsWith("[Gen"))
            {
                continue;
            }
            _formats.Add(format.Split(",")[0]);
        }
    }

    public string GetFormattedTier(string tier)
    {
        return _formats.FirstOrDefault(format => format.ToLowerAlphaNum() == tier);
    }
}