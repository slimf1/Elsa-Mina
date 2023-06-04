using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Templates.Profile;

public class ProfileViewModel : BaseViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Avatar { get; set; }
    public char UserRoomRank { get; set; }
    public string Status { get; set; }
    public string Title { get; set; }
    public IEnumerable<Badge> Badges { get; set; }
    public DateTimeOffset RegisterDate { get; set; }
}