namespace ElsaMina.Core.Services.Probabilities;

public class RandomService : IRandomService
{
    private Random _rng = new();

    public T RandomElement<T>(IEnumerable<T> enumerable)
    {
        var list = enumerable.ToList();
        return list.ElementAt(_rng.Next(0, list.Count));
    }

    public void ShuffleInPlace<T>(IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = _rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public double NextDouble()
    {
        return _rng.NextDouble();
    }

    public int NextInt(int lowerBound, int upperBound)
    {
        return _rng.Next(lowerBound, upperBound);
    }

    public void SetSeed(int seed)
    {
        _rng = new Random(seed);
    }
}