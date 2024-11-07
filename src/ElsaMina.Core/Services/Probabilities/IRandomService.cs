namespace ElsaMina.Core.Services.Probabilities;

public interface IRandomService
{
    T RandomElement<T>(IEnumerable<T> enumerable);
    double NextDouble();
    void ShuffleInPlace<T>(IList<T> list);
    int NextInt(int lowerBound, int upperBound);
}