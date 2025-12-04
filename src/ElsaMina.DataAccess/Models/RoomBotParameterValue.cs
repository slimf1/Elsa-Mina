using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("RoomBotParameterValues")]
public class RoomBotParameterValue
{
    public string RoomId { get; set; }
    public SavedRoom SavedRoom { get; set; }
    public string ParameterId { get; set; }
    public string Value { get; set; }
}