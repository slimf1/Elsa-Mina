namespace ElsaMina.Commands.VoltorbFlip;

public interface IVoltorbFlipGameManager
{
    IVoltorbFlipGame GetGame(string roomId, string userId);
    void RegisterGame(string roomId, string userId, IVoltorbFlipGame game);
    void RemoveGame(string roomId, string userId);
}
