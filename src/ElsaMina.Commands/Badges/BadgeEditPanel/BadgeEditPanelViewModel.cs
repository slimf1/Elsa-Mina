using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Badges.BadgeEditPanel;

public class BadgeEditPanelViewModel : LocalizableViewModel
{
    public string BotName { get; set; }
    public string Trigger { get; set; }
    public string RoomId { get; set; }
    public string RoomName { get; set; }
    public string EditCommand { get; set; }
    public IReadOnlyCollection<Badge> Badges { get; set; }
}
