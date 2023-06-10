using System.Text.RegularExpressions;
using ElsaMina.Core.Models;

namespace ElsaMina.Core.Utils;

public static class ShowdownTeams
{
    private static readonly Dictionary<string, string> BATTLE_STAT_IDS = new()
    {
        ["HP"] = "hp",
        ["hp"] = "hp",
        ["Atk"] = "atk",
        ["atk"] = "atk",
        ["Def"] = "def",
        ["def"] = "def",
        ["SpA"] = "spa",
        ["SAtk"] = "spa",
        ["SpAtk"] = "spa",
        ["spa"] = "spa",
        ["SpD"] = "spd",
        ["SDef"] = "spd",
        ["SpDef"] = "spd",
        ["spd"] = "spd",
        ["Spe"] = "spe",
        ["Spd"] = "spe",
        ["spe"] = "spe"
    };

    private static readonly Dictionary<string, string> BATTLE_STAT_NAMES = new()
    {
        ["hp"] = "HP",
        ["atk"] = "Atk",
        ["def"] = "Def",
        ["spa"] = "SpA",
        ["spd"] = "SpD",
        ["spe"] = "Spe"
    };

    private static readonly Regex NATURE_REGEX = new("^[A-Za-z]+ (N|n)ature");
    
    public static IEnumerable<PokemonSet> DeserializeTeamExport(string export)
    {
        var team = new List<PokemonSet>();
        PokemonSet currentSet = null;
        
        foreach (var teamLine in export.Split("\n"))
        {
            var line = teamLine.Trim();
            if (line == string.Empty || line == "---")
            {
                currentSet = null;
            } else if (currentSet == null) 
            {
                currentSet = new PokemonSet
                {
                    Name = string.Empty,
                    Species = string.Empty,
                    Gender = string.Empty
                };
                
                team.Add(currentSet);
                var atIndex = line.LastIndexOf(" @ ", StringComparison.Ordinal);
                if (atIndex != -1)
                {
                    currentSet.Item = line.Substring(atIndex + 3);
                    if (currentSet.Item.ToLowerAlphaNum() == "noitem")
                    {
                        currentSet.Item = string.Empty;
                    }

                    line = line.Substring(0, atIndex);
                }

                if (line.Substring(line.Length - 4) == " (M)")
                {
                    currentSet.Gender = "M";
                    line = line.Substring(0, line.Length - 4);
                }

                if (line.Substring(line.Length - 4) == " (F)")
                {
                    currentSet.Gender = "F";
                    line = line.Substring(0, line.Length - 4);
                }

                var parenIndex = line.LastIndexOf(" (", StringComparison.Ordinal);

                if (line.Substring(line.Length - 1) == ")" && parenIndex != -1)
                {
                    line = line.Substring(0, line.Length - 1);
                    currentSet.Species = line.Substring(parenIndex + 2); // TODO : dex
                    line = line.Substring(0, parenIndex);
                    currentSet.Name = line;
                }
                else
                {
                    currentSet.Species = line; // TODO : dex
                    currentSet.Name = string.Empty;
                }
            } else if (line.Length > 7 && line.Substring(0, 7) == "Trait: ")
            {
                line = line.Substring(7);
                currentSet.Ability = line;
            } else if (line.Length > 9 && line.Substring(0, 9) == "Ability: ")
            {
                line = line.Substring(9);
                currentSet.Ability = line;
            } else if (line == "Shiny: Yes")
            {
                currentSet.IsShiny = true;
            } else if (line.Length > 7 && line.Substring(0, 7) == "Level: ")
            {
                line = line.Substring(7);
                currentSet.Level = int.Parse(line);
            } else if (line.Length > 11 && line.Substring(0, 11) == "Happiness: ")
            {
                line = line.Substring(11);
                currentSet.Happiness = int.Parse(line);
            } else if (line.Length > 10 && line.Substring(0, 10) == "Pokeball: ")
            {
                line = line.Substring(10);
                currentSet.Pokeball = line;
            } else if (line.Length > 14 && line.Substring(0, 14) == "Hidden Power: ")
            {
                line = line.Substring(14);
                currentSet.HiddenPowerType = line;
            } else if (line.Length > 11 && line.Substring(0, 11) == "Tera Type: ")
            {
                line = line.Substring(11);
                currentSet.TeraType = line;
            } else if (line.Length > 15 && line.Substring(0, 15) == "Dynamax Level: ")
            {
                line = line.Substring(15);
                currentSet.DynamaxLevel = int.Parse(line);
            } else if (line == "Gigantamax: Yes")
            {
                currentSet.IsGigantamax = true;
            } else if (line.Length > 5 && line.Substring(0, 5) == "EVs: ")
            {
                line = line.Substring(5);
                var evLines = line.Split("/");
                currentSet.EffortValues = new Dictionary<string, int>
                {
                    ["hp"] = 0,
                    ["atk"] = 0,
                    ["def"] = 0,
                    ["spa"] = 0,
                    ["spd"] = 0,
                    ["spe"] = 0,
                };
                foreach (var untrimmedEvLine in evLines)
                {
                    var evLine = untrimmedEvLine.Trim();
                    var spaceIndex = evLine.IndexOf(' ');
                    if (spaceIndex == -1)
                    {
                        continue;
                    }

                    var statId = BATTLE_STAT_IDS[evLine.Substring(spaceIndex + 1)];
                    var statVal = int.Parse(evLine.Substring(0, spaceIndex));
                    if (string.IsNullOrEmpty(statId))
                    {
                        continue;
                    }

                    currentSet.EffortValues[statId] = statVal;
                }
            } else if (line.Substring(0, 5) == "IVs: ")
            {
                line = line.Substring(5);
                var ivLines = line.Split(" / ");
                currentSet.IndividualValues = new Dictionary<string, int>
                {
                    ["hp"] = 31,
                    ["atk"] = 31,
                    ["def"] = 31,
                    ["spa"] = 31,
                    ["spd"] = 31,
                    ["spe"] = 31,
                };
                foreach (var untrimmedIvLine in ivLines)
                {
                    var ivLine = untrimmedIvLine.Trim();
                    var spaceIndex = ivLine.IndexOf(' ');
                    if (spaceIndex == -1)
                    {
                        continue;
                    }
                    
                    var statId = BATTLE_STAT_IDS[ivLine.Substring(spaceIndex + 1)];
                    var statVal = int.Parse(ivLine.Substring(0, spaceIndex));
                    if (string.IsNullOrEmpty(statId))
                    {
                        continue;
                    }

                    currentSet.IndividualValues[statId] = statVal;
                }
            } else if (NATURE_REGEX.IsMatch(line))
            {
                var natureIndex = line.IndexOf(" Nature", StringComparison.Ordinal);
                if (natureIndex == -1)
                {
                    natureIndex = line.IndexOf(" nature", StringComparison.Ordinal);
                }

                if (natureIndex == -1)
                {
                    continue;
                }

                line = line.Substring(0, natureIndex);
                if (!string.IsNullOrEmpty(line))
                {
                    currentSet.Nature = line;
                }
            } else if (line.Substring(0, 1) == "-" || line.Substring(0, 1) == "~")
            {
                line = line.Substring(1);
                if (line.Substring(0, 1) == " ")
                {
                    line = line.Substring(1);
                }

                if (currentSet.Moves == null)
                {
                    currentSet.Moves = new List<string>();
                }

                currentSet.Moves.Add(line);
            }
        }
        
        return team;
    }

    public static string GetTeamExport(IEnumerable<PokemonSet> team)
    {
        return string.Empty;
    }
}