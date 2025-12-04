using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

/// <summary>
/// Room data independent of users
/// </summary>
[Table("Rooms")]
public class SavedRoom
{
    public string Id { get; set; }
    public string Title { get; set; }
    public ICollection<RoomTeam> Teams { get; set; } = new HashSet<RoomTeam>();
    public ICollection<RoomBotParameterValue> ParameterValues { get; set; } = new HashSet<RoomBotParameterValue>();
    public ICollection<SavedPoll> PollHistory { get; set; } = new HashSet<SavedPoll>();
    public ICollection<Badge> Badges { get; set; } = new HashSet<Badge>();
    public ICollection<AddedCommand> AddedCommands { get; set; } = new HashSet<AddedCommand>();
}