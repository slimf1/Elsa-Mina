namespace ElsaMina.Commands.FloodIt;

public interface IFloodItGameManager
{
    IFloodItGame GetGame(string roomId, string userId);
    void RegisterGame(string roomId, string userId, IFloodItGame game);
    void RemoveGame(string roomId, string userId);
}
