namespace ElsaMina.Commands.Games.LightsOut;

public interface ILightsOutGameManager
{
    ILightsOutGame GetGame(string roomId, string userId);
    void RegisterGame(string roomId, string userId, ILightsOutGame game);
    void RemoveGame(string roomId, string userId);
}
