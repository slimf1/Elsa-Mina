﻿using ElsaMina.Core.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Teams;

public class TeamPreviewViewModel : LocalizableViewModel
{
    public IEnumerable<PokemonSet> Team { get; init; }
    public string Sender { get; init; }
    public string Author { get; init; }
}