using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using ElsaMina.Logging;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Core.Services.Rooms.Parameters;

public class EfRoomParameterStore : IRoomParameterStore
{
    private readonly IBotDbContextFactory _dbContextFactory;
    private readonly IReadOnlyDictionary<Parameter, IParameterDefinition> _parameterDefinitions;
    private SavedRoom _dbSavedRoom;

    public EfRoomParameterStore(IBotDbContextFactory dbContextFactory, IParametersDefinitionFactory definitionFactory)
    {
        _dbContextFactory = dbContextFactory;
        _parameterDefinitions = definitionFactory.GetParametersDefinitions();
    }

    public IRoom Room { get; set; }

    public void InitializeFromRoomEntity(SavedRoom savedRoomEntity)
    {
        _dbSavedRoom = savedRoomEntity;
    }

    public Task<string> GetValueAsync(Parameter parameter, CancellationToken cancellationToken = default)
        => cancellationToken.IsCancellationRequested
            ? Task.FromCanceled<string>(cancellationToken)
            : Task.FromResult(GetCachedValue(parameter));

    private string GetCachedValue(Parameter parameter)
    {
        var parameterDefinition = _parameterDefinitions[parameter];
        return _dbSavedRoom
                   .ParameterValues
                   .FirstOrDefault(parameterValue => parameterValue.ParameterId == parameterDefinition.Identifier)
                   ?.Value
               ?? parameterDefinition.DefaultValue;
    }

    public async Task<bool> SetValueAsync(Parameter parameter, string value,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var parameterDefinition = _parameterDefinitions[parameter];

            var existing = _dbSavedRoom.ParameterValues
                .FirstOrDefault(parameterValue => parameterValue.ParameterId == parameterDefinition.Identifier);

            if (existing == null)
            {
                existing = new RoomBotParameterValue
                {
                    RoomId = _dbSavedRoom.Id,
                    ParameterId = parameterDefinition.Identifier,
                    Value = value
                };

                _dbSavedRoom.ParameterValues.Add(existing);
                dbContext.RoomBotParameterValues.Add(existing);
            }
            else
            {
                existing.Value = value;
                // L'instance n'est pas suivie par le même dbcontext
                dbContext.RoomBotParameterValues.Update(existing);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            parameterDefinition.OnUpdateAction?.Invoke(Room, value);
            return true;
        }
        catch (DbUpdateException ex)
        {
            Log.Error(ex,
                "Failed to set room parameter value for RoomId={RoomId}, Parameter={Parameter}",
                _dbSavedRoom.Id, parameter);

            return false;
        }
    }
}