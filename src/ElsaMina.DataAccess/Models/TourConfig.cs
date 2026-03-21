using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("TourConfigs")]
public class TourConfig
{
    public string Id { get; set; }
    public string RoomId { get; set; }
    public string Tier { get; set; }
    public string Format { get; set; }
    public int Autostart { get; set; }
    public int? AutoDq { get; set; }
    public string TourName { get; set; }
    public string Teams { get; set; }
    public string Rules { get; set; }
}
