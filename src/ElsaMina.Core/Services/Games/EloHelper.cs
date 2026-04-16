namespace ElsaMina.Core.Services.Games;

public static class EloHelper
{
    private const int K_FACTOR = 32;

    public const int DEFAULT_RATING = 1000;

    public static (int newWinnerRating, int newLoserRating) CalculateWinRatings(int winnerRating, int loserRating)
    {
        var (newWinner, newLoser) = CalculateNewRatings(winnerRating, loserRating, 1.0);
        return (newWinner, newLoser);
    }

    public static (int newRating1, int newRating2) CalculateDrawRatings(int rating1, int rating2)
    {
        return CalculateNewRatings(rating1, rating2, 0.5);
    }

    private static (int newRating1, int newRating2) CalculateNewRatings(int rating1, int rating2, double score1)
    {
        var expected1 = 1.0 / (1.0 + Math.Pow(10.0, (rating2 - rating1) / 400.0));
        var expected2 = 1.0 - expected1;
        var score2 = 1.0 - score1;

        var newRating1 = (int)Math.Round(rating1 + K_FACTOR * (score1 - expected1));
        var newRating2 = (int)Math.Round(rating2 + K_FACTOR * (score2 - expected2));

        return (newRating1, newRating2);
    }
}
