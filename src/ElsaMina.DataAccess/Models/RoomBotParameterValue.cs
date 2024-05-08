using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("RoomBotParameterValues")]
public class RoomBotParameterValue : IKeyed<Tuple<string, string>>
{
    public Tuple<string, string> Key => new(RoomId, ParameterId);

    public string RoomId { get; set; }
    public RoomParameters RoomParameters { get; set; }
    public string ParameterId { get; set; }
    public string Value { get; set; }
}