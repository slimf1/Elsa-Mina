using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Commands.Arcade.Events;

public class EventRoleMappingService : IEventRoleMappingService
{
    private readonly IBotDbContextFactory _dbContextFactory;

    public EventRoleMappingService(IBotDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IReadOnlyList<EventRoleMapping>> GetMappingsForRoomAsync(string roomId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.EventRoleMappings
            .Where(mapping => mapping.RoomId == roomId)
            .OrderBy(mapping => mapping.EventName)
            .ToListAsync(cancellationToken);
    }

    public async Task<EventRoleMapping> GetMappingAsync(string eventName, string roomId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.EventRoleMappings.FindAsync([eventName, roomId], cancellationToken);
    }

    public async Task SaveMappingAsync(EventRoleMapping mapping,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await dbContext.EventRoleMappings.FindAsync([mapping.EventName, mapping.RoomId], cancellationToken);
        if (existing == null)
        {
            await dbContext.EventRoleMappings.AddAsync(mapping, cancellationToken);
        }
        else
        {
            existing.DiscordRoleId = mapping.DiscordRoleId;
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMappingAsync(string eventName, string roomId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await dbContext.EventRoleMappings.FindAsync([eventName, roomId], cancellationToken);
        if (existing != null)
        {
            dbContext.EventRoleMappings.Remove(existing);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
