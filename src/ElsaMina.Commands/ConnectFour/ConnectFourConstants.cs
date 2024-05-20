namespace ElsaMina.Commands.ConnectFour;

public static class ConnectFourConstants
{
    public const int GRID_HEIGHT = 6;
    public const int GRID_WIDTH = 7;
    public const int MAX_PLAYERS_COUNT = 2;
    public const int WINNING_LENGTH = 4;
    public static readonly TimeSpan TIMEOUT_DELAY = TimeSpan.FromSeconds(30);
    public static readonly char[] SYMBOLS = ['X', '0'];
}