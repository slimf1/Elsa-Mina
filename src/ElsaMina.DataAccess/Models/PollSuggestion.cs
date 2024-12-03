using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElsaMina.DataAccess.Models;

[Table("PollSuggestions")]
public class PollSuggestion : IKeyed<int>
{
    public int Key => Id;
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(30)]
    public string RoomId { get; set; }
    
    [MaxLength(20)]
    public string Username { get; set; }
    
    [MaxLength(300)]
    public string Suggestion { get; set; }
}