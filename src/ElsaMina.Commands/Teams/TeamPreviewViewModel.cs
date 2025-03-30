﻿using ElsaMina.Core.Models;
using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Teams;

public class TeamPreviewViewModel : LocalizableViewModel
{
    public IEnumerable<PokemonSet> Team { get; init; }
    public string Sender { get; init; }
    public string Author { get; init; }
}