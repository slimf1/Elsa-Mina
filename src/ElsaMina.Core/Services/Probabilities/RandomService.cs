namespace ElsaMina.Core.Services.Probabilities;

public class RandomService : IRandomService
{
    private Random _rng = new();

    public T RandomElement<T>(IEnumerable<T> enumerable)
    {
        var list = enumerable.ToList();
        return list.ElementAt(_rng.Next(0, list.Count));
    }

    public double NextDouble()
    {
        return _rng.NextDouble();
    }

    public void SetSeed(int seed)
    {
        _rng = new Random(seed);
    }
}