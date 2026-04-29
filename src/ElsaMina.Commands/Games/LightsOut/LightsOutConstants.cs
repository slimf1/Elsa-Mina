namespace ElsaMina.Commands.Games.LightsOut;

public static class LightsOutConstants
{
    public const int MAX_LEVEL = 15;
    public static readonly TimeSpan INACTIVITY_TIMEOUT = TimeSpan.FromMinutes(2);

    // (GridSize, Presses) per level
    public static readonly (int GridSize, int Presses)[] LEVEL_CONFIGURATIONS =
    [
        (5, 3),   // Level 1
        (5, 4),   // Level 2
        (5, 5),   // Level 3
        (5, 6),   // Level 4
        (5, 7),   // Level 5
        (6, 5),   // Level 6
        (6, 7),   // Level 7
        (6, 9),   // Level 8
        (7, 7),   // Level 9
        (7, 10),  // Level 10
        (7, 12),  // Level 11
        (8, 10),  // Level 12
        (8, 14),  // Level 13
        (8, 18),  // Level 14
        (8, 22)   // Level 15
    ];

    public static (int GridSize, int Presses) GetLevelConfig(int level)
    {
        var index = Math.Clamp(level - 1, 0, LEVEL_CONFIGURATIONS.Length - 1);
        return LEVEL_CONFIGURATIONS[index];
    }
}
