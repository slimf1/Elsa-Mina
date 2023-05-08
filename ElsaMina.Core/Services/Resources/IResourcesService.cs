using System.Globalization;

namespace ElsaMina.Core.Services.Resources;

public interface IResourcesService
{
    public IEnumerable<CultureInfo> SupportedLocales { get; }
    string GetString(string key, CultureInfo cultureInfo = null);
}