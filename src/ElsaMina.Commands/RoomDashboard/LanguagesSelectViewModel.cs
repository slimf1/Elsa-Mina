using System.Globalization;
using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.RoomDashboard;

public class LanguagesSelectViewModel : LocalizableViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<CultureInfo> Cultures { get; set; }
}