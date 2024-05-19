namespace ElsaMina.Core.Services.Probabilities;

public interface IRandomService
{
    T RandomElement<T>(IEnumerable<T> enumerable);
    double NextDouble();
    void SetSeed(int seed);
    void ShuffleInPlace<T>(IList<T> list);
}