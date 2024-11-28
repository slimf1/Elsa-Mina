namespace ElsaMina.Core.Utils;

public static class EnumerableExtensions
{
    public static IEnumerable<(int index, T value)> Enumerate<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null)
        {
            yield break;
        }
        var index = 0;
        foreach (var item in enumerable)
        {
            yield return (index++, item);
        }
    }
}