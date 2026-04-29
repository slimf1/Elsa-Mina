namespace ElsaMina.Commands.Games.GuessingGame;

public interface IGuessingGame
{
    void OnAnswer(string userName, string answer);
    void StopGame();
}