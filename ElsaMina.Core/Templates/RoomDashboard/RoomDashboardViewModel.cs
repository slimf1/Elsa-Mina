using ElsaMina.Core.Templates.LanguageSelect;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Templates.RoomDashboard;

public class RoomDashboardViewModel : BaseViewModel
{
    public string BotName { get; set; }
    public string Trigger { get; set; }
    public RoomParameters RoomParameters { get; set; }
    public string RoomName { get; set; }
    public LanguagesSelectViewModel LanguageSelectModel { get; set; }
}