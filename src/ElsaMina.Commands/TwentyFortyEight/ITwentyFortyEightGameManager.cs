namespace ElsaMina.Commands.TwentyFortyEight;

public interface ITwentyFortyEightGameManager
{
    ITwentyFortyEightGame GetGame(string roomId, string userId);
    void RegisterGame(string roomId, string userId, ITwentyFortyEightGame game);
    void RemoveGame(string roomId, string userId);
}
