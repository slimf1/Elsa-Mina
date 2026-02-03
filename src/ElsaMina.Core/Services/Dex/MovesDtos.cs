namespace ElsaMina.Core.Services.Dex;

using Newtonsoft.Json;

public sealed class MoveData
{
    [JsonProperty("num")]
    public int Num { get; set; }

    [JsonProperty("accuracy")]
    public object Accuracy { get; set; } = true;

    [JsonProperty("basePower")]
    public int BasePower { get; set; }

    [JsonProperty("category")]
    public string Category { get; set; } = string.Empty;

    [JsonProperty("isNonstandard")]
    public string IsNonstandard { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("pp")]
    public int Pp { get; set; }

    [JsonProperty("priority")]
    public int Priority { get; set; }

    [JsonProperty("flags")]
    public Dictionary<string, int> Flags { get; set; } = new();

    [JsonProperty("isZ")]
    public string IsZ { get; set; } = string.Empty;

    [JsonProperty("critRatio")]
    public int CritRatio { get; set; }

    [JsonProperty("secondary")]
    public SecondaryEffect Secondary { get; set; } = new();

    [JsonProperty("target")]
    public string Target { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("contestType")]
    public string ContestType { get; set; } = string.Empty;

    [JsonProperty("boosts")]
    public StatBoosts Boosts { get; set; } = new();

    [JsonProperty("drain")]
    public int[] Drain { get; set; } = Array.Empty<int>();

    [JsonProperty("zMove")]
    public ZMoveInfo ZMove { get; set; } = new();
}

public sealed class SecondaryEffect
{
    [JsonProperty("chance")]
    public int Chance { get; set; }

    [JsonProperty("boosts")]
    public StatBoosts Boosts { get; set; } = new();
}

public sealed class StatBoosts
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

    [JsonProperty("accuracy")]
    public int Accuracy { get; set; }

    [JsonProperty("evasion")]
    public int Evasion { get; set; }
}

public sealed class ZMoveInfo
{
    [JsonProperty("effect")]
    public string Effect { get; set; } = string.Empty;
}
