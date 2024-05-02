using System.Globalization;

namespace ElsaMina.Core.Contexts;

public interface IContextProvider
{
    IEnumerable<string> CurrentWhitelist { get; }
    string DefaultRoom { get; }
    CultureInfo DefaultCulture { get; }
    string GetString(string key, CultureInfo culture);
}