namespace ElsaMina.Core.Services.Probabilities;

public class RandomService : IRandomService
{
    private static readonly Random RNG = new();

    public T RandomElement<T>(IEnumerable<T> enumerable)
    {
        var list = enumerable.ToList();
        return list.ElementAt(RNG.Next(0, list.Count));
    }

    public double NextDouble()
    {
        return RNG.NextDouble();
    }
}