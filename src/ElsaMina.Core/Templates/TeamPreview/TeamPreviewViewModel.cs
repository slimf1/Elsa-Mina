using ElsaMina.Core.Models;

namespace ElsaMina.Core.Templates.TeamPreview;

public class TeamPreviewViewModel : LocalizableViewModel
{
    public IEnumerable<PokemonSet> Team { get; set; }
    public string Sender { get; set; }
    public string Author { get; set; }
}