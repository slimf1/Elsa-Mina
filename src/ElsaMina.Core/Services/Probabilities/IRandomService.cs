namespace ElsaMina.Core.Services.Probabilities;

public interface IRandomService
{
    T RandomElement<T>(IEnumerable<T> enumerable);
    IEnumerable<T> RandomSample<T>(IEnumerable<T> enumerable, int count);
    double NextDouble();
    void ShuffleInPlace<T>(IList<T> list);
    int NextInt(int lowerBound, int upperBound);
    int NextInt(int upperBound);
}