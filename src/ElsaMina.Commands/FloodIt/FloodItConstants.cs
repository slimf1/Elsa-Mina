namespace ElsaMina.Commands.FloodIt;

public static class FloodItConstants
{
    public const int MAX_LEVEL = 10;
    public static readonly TimeSpan INACTIVITY_TIMEOUT = TimeSpan.FromMinutes(3);

    public static readonly string[] COLOR_HEX =
    [
        "#E74C3C", // Red
        "#E67E22", // Orange
        "#F1C40F", // Yellow
        "#2ECC71", // Green
        "#3498DB", // Blue
        "#9B59B6"  // Purple
    ];

    // (GridSize, ColorCount, TargetMoves, MaxMoves) per level
    // TargetMoves = optimal threshold (3 stars); MaxMoves = hard limit (loss if exceeded)
    public static readonly (int GridSize, int ColorCount, int TargetMoves, int MaxMoves)[] LEVEL_CONFIGURATIONS =
    [
        (8,  4, 13, 17), // Level 1
        (8,  5, 16, 22), // Level 2
        (10, 5, 21, 28), // Level 3
        (10, 6, 25, 34), // Level 4
        (10, 6, 27, 38), // Level 5
        (12, 6, 28, 38), // Level 6
        (12, 6, 32, 44), // Level 7
        (14, 6, 30, 41), // Level 8
        (14, 6, 37, 52), // Level 9
        (14, 6, 43, 62), // Level 10
    ];

    public static (int GridSize, int ColorCount, int TargetMoves, int MaxMoves) GetLevelConfig(int level)
    {
        var index = Math.Clamp(level - 1, 0, LEVEL_CONFIGURATIONS.Length - 1);
        return LEVEL_CONFIGURATIONS[index];
    }
}
