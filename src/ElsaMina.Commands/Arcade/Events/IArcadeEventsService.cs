namespace ElsaMina.Commands.Arcade.Events;

public interface IArcadeEventsService
{
    void MuteGames(string roomId, TimeSpan duration);
    void UnmuteGames(string roomId);
    bool AreGamesMuted(string roomId);
}
