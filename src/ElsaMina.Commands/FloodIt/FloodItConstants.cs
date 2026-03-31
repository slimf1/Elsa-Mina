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
    // MaxMoves = TargetMoves + leeway, where leeway grows slightly with level:
    //   levels 1-3: leeway 2 | levels 4-6: leeway 3 | levels 7-10: leeway 4
    public static readonly (int GridSize, int ColorCount, int TargetMoves, int MaxMoves)[] LEVEL_CONFIGURATIONS =
    [
        (8,  4, 10, 12), // Level 1
        (8,  5, 12, 14), // Level 2
        (10, 5, 16, 18), // Level 3
        (10, 6, 19, 22), // Level 4
        (10, 6, 21, 24), // Level 5
        (12, 6, 23, 26), // Level 6
        (12, 6, 25, 29), // Level 7
        (14, 6, 27, 31), // Level 8
        (14, 6, 30, 34), // Level 9
        (14, 6, 34, 38), // Level 10
    ];

    public static (int GridSize, int ColorCount, int TargetMoves, int MaxMoves) GetLevelConfig(int level)
    {
        var index = Math.Clamp(level - 1, 0, LEVEL_CONFIGURATIONS.Length - 1);
        return LEVEL_CONFIGURATIONS[index];
    }
}
