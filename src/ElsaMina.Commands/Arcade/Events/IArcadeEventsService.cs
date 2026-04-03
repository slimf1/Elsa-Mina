namespace ElsaMina.Commands.Arcade.Events;

public interface IArcadeEventsService
{
    void MuteGames(string roomId, TimeSpan duration);
    bool AreGamesMuted(string roomId);
}
