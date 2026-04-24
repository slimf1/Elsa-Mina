namespace ElsaMina.Commands.Tournaments.Betting;

public enum BetPlacementError
{
    Success,
    NoBettingSession,
    BettingClosed,
    InvalidPlayer,
    AlreadyBet
}