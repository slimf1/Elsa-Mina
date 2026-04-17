using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Badges.UserBadgePanel;

public class UserBadgePanelViewModel : LocalizableViewModel
{
    public string BotName { get; set; }
    public string Trigger { get; set; }
    public string RoomId { get; set; }
    public string RoomName { get; set; }
    public string TargetUserId { get; set; }
    public IReadOnlyCollection<Badge> AllBadges { get; set; }
    public IReadOnlySet<string> OwnedBadgeIds { get; set; }
}
