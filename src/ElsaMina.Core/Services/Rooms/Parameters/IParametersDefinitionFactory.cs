namespace ElsaMina.Core.Services.Rooms.Parameters;

public interface IParametersDefinitionFactory
{
    IReadOnlyDictionary<Parameter, IParameterDefinition> GetParametersDefinitions();
}