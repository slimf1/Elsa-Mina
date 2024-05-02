using ElsaMina.Core.Models;
using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Teams;

public class TeamPreviewViewModel : LocalizableViewModel
{
    public IEnumerable<PokemonSet> Team { get; set; }
    public string Sender { get; set; }
    public string Author { get; set; }
}