namespace ElsaMina.Commands.GuessingGame;

public interface IGuessingGame
{
    void OnAnswer(string userName, string answer);
    void Cancel();
}