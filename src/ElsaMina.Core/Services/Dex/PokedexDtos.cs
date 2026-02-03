using Newtonsoft.Json;

namespace ElsaMina.Core.Services.Dex;

public sealed class PokedexEntry
{
    [JsonProperty("num")]
    public int Num { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("types")]
    public string[] Types { get; set; }

    [JsonProperty("baseStats")]
    public BaseStats BaseStats { get; set; }

    [JsonProperty("abilities")]
    public Dictionary<string, string> Abilities { get; set; }

    [JsonProperty("heightm")]
    public double? Heightm { get; set; }

    [JsonProperty("weightkg")]
    public double? Weightkg { get; set; }

    [JsonProperty("color")]
    public string Color { get; set; }

    [JsonProperty("evos")]
    public string[] Evos { get; set; }

    [JsonProperty("prevo")]
    public string Prevo { get; set; }

    [JsonProperty("evoLevel")]
    public int EvoLevel { get; set; }

    [JsonProperty("evoType")]
    public string EvoType { get; set; }

    [JsonProperty("evoItem")]
    public string EvoItem { get; set; }

    [JsonProperty("evoCondition")]
    public string EvoCondition { get; set; }

    [JsonProperty("eggGroups")]
    public string[] EggGroups { get; set; }

    // Formes / variants
    [JsonProperty("baseSpecies")]
    public string BaseSpecies { get; set; }

    [JsonProperty("forme")]
    public string Forme { get; set; }

    [JsonProperty("otherFormes")]
    public string[] OtherFormes { get; set; }

    [JsonProperty("formeOrder")]
    public string[] FormeOrder { get; set; }

    [JsonProperty("gender")]
    public string Gender { get; set; }

    [JsonProperty("gen")]
    public int Gen { get; set; }

    [JsonProperty("requiredItem")]
    public string RequiredItem { get; set; }
}

public sealed class BaseStats
{
    [JsonProperty("hp")]
    public int Hp { get; set; }

    [JsonProperty("atk")]
    public int Atk { get; set; }

    [JsonProperty("def")]
    public int Def { get; set; }

    [JsonProperty("spa")]
    public int Spa { get; set; }

    [JsonProperty("spd")]
    public int Spd { get; set; }

    [JsonProperty("spe")]
    public int Spe { get; set; }
}
