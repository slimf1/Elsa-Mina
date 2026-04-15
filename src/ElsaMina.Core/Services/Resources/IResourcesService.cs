using System.Globalization;

namespace ElsaMina.Core.Services.Resources;

public interface IResourcesService
{
    IEnumerable<CultureInfo> SupportedCultures { get; }
    string GetString(string key, CultureInfo cultureInfo = null);
}