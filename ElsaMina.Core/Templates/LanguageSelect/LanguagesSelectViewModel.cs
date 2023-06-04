using System.Globalization;

namespace ElsaMina.Core.Templates.LanguageSelect;

public class LanguagesSelectViewModel : LocalizableViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<CultureInfo> Cultures { get; set; }
}