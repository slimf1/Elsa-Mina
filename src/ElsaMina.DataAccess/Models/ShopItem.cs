using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("ShopItems")]
public class ShopItem
{
    public int Id { get; set; }
    public string Tier { get; set; }
    public string Article { get; set; }
    public string Price { get; set; }
}
