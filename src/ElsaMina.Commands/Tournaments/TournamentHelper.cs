using ElsaMina.Core.Utils;
using Newtonsoft.Json;

namespace ElsaMina.Commands.Tournaments;

public static class TournamentHelper
{
    public static Dictionary<string, int> ParseTourTree(TournamentNode node)
    {
        var auxobj = new Dictionary<string, int>();
        var team = node.Team;
        var state = node.State;
        var children = node.Children ?? new List<TournamentNode>();

        if (team != null)
        {
            auxobj.TryAdd(team, 0);
        }

        if (!string.IsNullOrEmpty(state) && state == "finished" && team != null)
        {
            auxobj[team]++;
        }

        foreach (var child in children)
        {
            var aux = ParseTourTree(child);
            foreach (var kvp in aux)
            {
                auxobj.TryAdd(kvp.Key, 0);
                auxobj[kvp.Key] += kvp.Value;
            }
        }

        return auxobj;
    }

    public static Dictionary<string, object> ParseTourResults(string jsonData)
    {
        var data = JsonConvert.DeserializeObject<TournamentData>(jsonData);

        if (data.Generator.ToLowerAlphaNum() != "singleelimination")
        {
            return null;
        }

        var res = new Dictionary<string, object>();
        var parsedTree = ParseTourTree(data.BracketData.RootNode);

        res["players"] = new List<string>(parsedTree.Keys);

        var general = new Dictionary<string, int>();
        foreach (var key in parsedTree.Keys)
        {
            general[key.ToLowerAlphaNum()] = parsedTree[key];
        }

        res["general"] = general;

        res["winner"] = data.Results[0][0].ToLowerAlphaNum();
        res["finalist"] = string.Empty;
        res["semiFinalists"] = new List<string>();

        if (data.BracketData.RootNode.Children != null)
        {
            foreach (var child in data.BracketData.RootNode.Children)
            {
                var aux = child.Team?.ToLowerAlphaNum() ?? string.Empty;
                if (!string.IsNullOrEmpty(aux) && aux != (string)res["winner"])
                {
                    res["finalist"] = aux;
                }

                if (child.Children != null)
                {
                    foreach (var grandchild in child.Children)
                    {
                        var aux2 = grandchild.Team.ToLowerAlphaNum() ?? string.Empty;
                        if (!string.IsNullOrEmpty(aux2) && aux2 != (string)res["winner"] &&
                            aux2 != (string)res["finalist"] && !((List<string>)res["semiFinalists"]).Contains(aux2))
                        {
                            ((List<string>)res["semiFinalists"]).Add(aux2);
                        }
                    }
                }
            }
        }

        return res;
    }
}