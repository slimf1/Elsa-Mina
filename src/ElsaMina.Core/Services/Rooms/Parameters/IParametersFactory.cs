namespace ElsaMina.Core.Services.Rooms.Parameters;

public interface IParametersFactory
{
    IReadOnlyDictionary<string, IParameter> GetParameters();
}