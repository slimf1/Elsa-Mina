using System.Globalization;

namespace ElsaMina.Core.Services.Resources;

public interface IResourcesService
{
    string GetString(string key, CultureInfo cultureInfo = null);
}