namespace ElsaMina.Core.Services.Elo;

public interface IEloService
{
    int DefaultRating { get; }

    (int newWinnerRating, int newLoserRating) CalculateWinRatings(int winnerRating, int loserRating);

    (int newRating1, int newRating2) CalculateDrawRatings(int rating1, int rating2);
}