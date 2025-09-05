namespace ElsaMina.Core.Utils;

public static class DoubleExtensions
{
    public static bool IsApproximatelyEqualTo(this double value, double other, double tolerance = 0.0001)
    {
        return Math.Abs(value - other) <= tolerance;
    }
}