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

    public static TournamentResults ParseTourResults(string jsonData)
    {
        var data = JsonConvert.DeserializeObject<TournamentData>(jsonData);

        if (data.Generator.ToLowerAlphaNum() != "singleelimination")
        {
            return null;
        }

        var parsedTree = ParseTourTree(data.BracketData.RootNode);
        var result = new TournamentResults();
        
        result.Players = parsedTree.Keys.ToList();

        var general = new Dictionary<string, int>();
        foreach (var key in parsedTree.Keys)
        {
            general[key.ToLowerAlphaNum()] = parsedTree[key];
        }

        result.General = general;

        result.Winner = data.Results[0][0].ToLowerAlphaNum();
        result.Finalist = string.Empty;
        result.SemiFinalists = [];

        if (data.BracketData.RootNode.Children != null)
        {
            foreach (var child in data.BracketData.RootNode.Children)
            {
                var aux = child.Team?.ToLowerAlphaNum() ?? string.Empty;
                if (!string.IsNullOrEmpty(aux) && aux != result.Winner)
                {
                    result.Finalist = aux;
                }

                if (child.Children != null)
                {
                    foreach (var grandchild in child.Children)
                    {
                        var aux2 = grandchild.Team.ToLowerAlphaNum() ?? string.Empty;
                        if (!string.IsNullOrEmpty(aux2) && aux2 != result.Winner &&
                            aux2 != result.Finalist && !result.SemiFinalists.Contains(aux2))
                        {
                            result.SemiFinalists.Add(aux2);
                        }
                    }
                }
            }
        }

        return result;
    }
}