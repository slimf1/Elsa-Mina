namespace ElsaMina.Core.Services.Probabilities;

public interface IRandomService
{
    T RandomElement<T>(IEnumerable<T> enumerable);
    double NextDouble();
}