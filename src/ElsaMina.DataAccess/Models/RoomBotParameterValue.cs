using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("RoomBotParameterValues")]
public class RoomBotParameterValue
{
    public string RoomId { get; set; }
    public Room Room { get; set; }
    public string ParameterId { get; set; }
    public string Value { get; set; }
}