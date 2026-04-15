using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.ConnectFour;

public record ConnectFourRatingChange(int OldRating, int NewRating)
{
    public int Delta => NewRating - OldRating;
}

public interface IConnectFourRatingService
{
    Task<(ConnectFourRatingChange, ConnectFourRatingChange)> UpdateRatingsOnWinAsync(IUser winner, IUser loser, CancellationToken cancellationToken = default);

    Task<(ConnectFourRatingChange, ConnectFourRatingChange)> UpdateRatingsOnDrawAsync(IUser player1, IUser player2, CancellationToken cancellationToken = default);
}
