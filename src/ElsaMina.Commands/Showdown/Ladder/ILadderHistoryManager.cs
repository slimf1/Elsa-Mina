namespace ElsaMina.Commands.Showdown.Ladder;

public interface ILadderHistoryManager
{
    IReadOnlyDictionary<string, int> GetPreviousEntriesAndSave(string ladderId,
        IEnumerable<LadderPlayerDto> players);

    IReadOnlyDictionary<string, int> GetPreviousPlacementsAndSave(string ladderId,
        IEnumerable<LadderPlayerDto> players);

    IReadOnlyDictionary<string, int> GetPreviousPrefixedPlacementsAndSave(string ladderId, string prefix,
        IEnumerable<LadderPlayerDto> players);
}
