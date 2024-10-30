namespace ElsaMina.Commands.Replays;

public static class ReplaysHelper
{
    public static IDictionary<string, IList<string>> GetTeamsFromLog(string replayLog)
    {
        var teams = new Dictionary<string, IList<string>>();
        foreach (var line in replayLog.Split('\n'))
        {
            var values = line.Split('|').Skip(1).ToArray();
            if (values.Length == 0 || values[0] != "poke")
            {
                continue;
            }
            var playerId = values[1];
            var species = values[2].Split(',')[0];
            if (teams.TryGetValue(playerId, out var team))
            {
                team.Add(species);
            }
            else
            {
                teams[playerId] = [species];
            }
        }
        
        return teams;
    }
}