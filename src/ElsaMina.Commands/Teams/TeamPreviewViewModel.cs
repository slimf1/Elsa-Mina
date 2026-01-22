using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Teams;

public class TeamPreviewViewModel : LocalizableViewModel
{
    public IEnumerable<PokemonSet> Team { get; init; }
    public string Sender { get; init; }
    public string Author { get; init; }
}