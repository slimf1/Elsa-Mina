using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Dex;

public class Name
{
    [JsonProperty("fr")]
    public string French { get; set; }

    [JsonProperty("en")]
    public string English { get; set; }

    [JsonProperty("jp")]
    public string Japanese { get; set; }
}

public class Sprite
{
    [JsonProperty("regular")]
    public string Regular { get; set; }

    [JsonProperty("shiny")]
    public string Shiny { get; set; }
}

public class PokemonType
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("image")]
    public string Image { get; set; }
}

public class Talent
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("tc")]
    public bool IsHidden { get; set; }
}

public class Stats
{
    [JsonProperty("hp")]
    public int HP { get; set; }

    [JsonProperty("atk")]
    public int Attack { get; set; }

    [JsonProperty("def")]
    public int Defense { get; set; }

    [JsonProperty("spe_atk")]
    public int SpecialAttack { get; set; }

    [JsonProperty("spe_def")]
    public int SpecialDefense { get; set; }

    [JsonProperty("vit")]
    public int Speed { get; set; }
}

public class Resistance
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("multiplier")]
    public double Multiplier { get; set; }
}

public class EvolutionNext
{
    [JsonProperty("pokedex_id")]
    public int PokedexId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("condition")]
    public string Condition { get; set; }
}

public class Evolution
{
    [JsonProperty("pre")]
    public object PreEvolution { get; set; }

    [JsonProperty("next")]
    public List<EvolutionNext> NextEvolutions { get; set; }

    [JsonProperty("mega")]
    public object MegaEvolution { get; set; }
}

public class Gender
{
    [JsonProperty("male")]
    public double Male { get; set; }

    [JsonProperty("female")]
    public double Female { get; set; }
}

public class Pokemon
{
    [JsonProperty("pokedex_id")]
    public int PokedexId { get; set; }

    [JsonProperty("generation")]
    public int Generation { get; set; }

    [JsonProperty("category")]
    public string Category { get; set; }

    [JsonProperty("name")]
    public Name Name { get; set; }

    [JsonProperty("sprites")]
    public Sprite Sprites { get; set; }

    [JsonProperty("types")]
    public List<PokemonType> Types { get; set; }

    [JsonProperty("talents")]
    public List<Talent> Talents { get; set; }

    [JsonProperty("stats")]
    public Stats Stats { get; set; }

    [JsonProperty("resistances")]
    public List<Resistance> Resistances { get; set; }

    [JsonProperty("evolution")]
    public Evolution Evolution { get; set; }

    [JsonProperty("height")]
    public string Height { get; set; }

    [JsonProperty("weight")]
    public string Weight { get; set; }

    [JsonProperty("egg_groups")]
    public List<string> EggGroups { get; set; }

    [JsonProperty("sexe")]
    public Gender Gender { get; set; }

    [JsonProperty("catch_rate", NullValueHandling = NullValueHandling.Ignore)]
    public int CatchRate { get; set; }

    [JsonProperty("level_100", NullValueHandling = NullValueHandling.Ignore)]
    public int Level100Experience { get; set; }

    [JsonProperty("formes")]
    public object Formes { get; set; }
}
