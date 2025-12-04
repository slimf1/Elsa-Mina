using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Badges.HallOfFame;

public class PlayerRecord
{
    public string UserId { get; set; }
    public List<Badge> Badges { get; set; }
    public int Total { get; set; }
    public int Solo { get; set; }
    public int Team { get; set; }
};