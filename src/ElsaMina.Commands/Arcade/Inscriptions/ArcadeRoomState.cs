namespace ElsaMina.Commands.Arcade.Inscriptions;

public class ArcadeRoomState
{
    public bool IsActive { get; set; }
    public HashSet<string> Participants { get; } = [];
    public HashSet<string> BannedUsers { get; } = [];
    public bool IsTimerExpired { get; set; }
    public DateTimeOffset? TimerEnd { get; set; }
    public string Title { get; set; } = "Tournoi Arcade";
    public CancellationTokenSource TimerCts { get; set; }
}