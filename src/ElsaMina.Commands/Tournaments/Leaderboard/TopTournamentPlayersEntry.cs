namespace ElsaMina.Commands.Tournaments.Leaderboard;

public record TopTournamentPlayersEntry(
    int Rank,
    string UserId,
    string UserName,
    int WinsCount,
    int RunnerUpCount,
    int ThirdPlaceCount,
    int TournamentsEnteredCount,
    int WonGames,
    int PlayedGames);
