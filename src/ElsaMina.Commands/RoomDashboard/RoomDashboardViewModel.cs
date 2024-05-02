using ElsaMina.Core.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.RoomDashboard;

public class RoomDashboardViewModel : LocalizableViewModel
{
    public string BotName { get; set; }
    public string Trigger { get; set; }
    public RoomParameters RoomParameters { get; set; }
    public string RoomName { get; set; }
    public LanguagesSelectViewModel LanguageSelectModel { get; set; }
}