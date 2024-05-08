using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("Users")]
public class User : IKeyed<string>
{
    public string Key => Id;

    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime? RegDate { get; set; }
}