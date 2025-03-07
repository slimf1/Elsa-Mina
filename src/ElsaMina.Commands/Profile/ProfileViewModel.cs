using ElsaMina.Commands.Showdown.Ranking;
using ElsaMina.Core.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Profile;

public class ProfileViewModel : LocalizableViewModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Avatar { get; set; }
    public char UserRoomRank { get; set; }
    public string Status { get; set; }
    public string Title { get; set; }
    public IEnumerable<Badge> Badges { get; set; }
    public DateTimeOffset RegisterDate { get; set; }
    public RankingDataDto BestRanking { get; set; }
}