using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Services.Formats;

public class FormatsManager : IFormatsManager
{
    private readonly SortedSet<string> _formats = [];

    public IEnumerable<string> Formats => _formats;

    public void ParseFormats(string[] formats)
    {
        if (formats == null)
        {
            return;
        }
        foreach (var format in formats)
        {
            if (!format.StartsWith("[Gen"))
            {
                continue;
            }
            _formats.Add(format.Split(",")[0]);
        }
    }

    public string GetCleanFormat(string formatId)
    {
        return _formats.FirstOrDefault(format => format.ToLowerAlphaNum() == formatId) ?? formatId;
    }
}