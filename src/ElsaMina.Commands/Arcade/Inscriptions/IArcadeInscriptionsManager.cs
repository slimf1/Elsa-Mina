namespace ElsaMina.Commands.Arcade.Inscriptions;

public interface IArcadeInscriptionsManager
{
    bool TryGetState(string roomId, out ArcadeRoomState state);
    bool HasActiveInscriptions(string roomId);
    ArcadeRoomState InitInscriptions(string roomId, string title);
    void StopInscriptions(string roomId);
    void StartTimer(string roomId, int minutes);
    void CancelTimer(string roomId);
}