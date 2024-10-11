using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class RoomBotParameterValueRepository : IRoomBotParameterValueRepository
{
    private readonly DbContext _dbContext;
    private bool _disposed;

    public RoomBotParameterValueRepository() : this(new BotDbContext())
    {
        
    }
    
    public RoomBotParameterValueRepository(DbContext context)
    {
        _dbContext = context;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    private void Dispose(bool disposing)
    {
        if (!disposing || _disposed)
        {
            return;
        }
        _dbContext?.Dispose();
        _disposed = true;
    } 

    public async Task<RoomBotParameterValue> GetByIdAsync(Tuple<string, string> key)
    {
        var (roomId, parameterId) = key;
        return await _dbContext.Set<RoomBotParameterValue>()
            .AsNoTracking()
            .Include(x => x.RoomParameters)
            .ThenInclude(x => x.ParameterValues)
            .FirstOrDefaultAsync(x => x.RoomId == roomId && x.ParameterId == parameterId);
    }

    public async Task<IEnumerable<RoomBotParameterValue>> GetAllAsync()
    {
        return await _dbContext.Set<RoomBotParameterValue>()
            .AsNoTracking()
            .Include(x => x.RoomParameters)
            .ThenInclude(x => x.ParameterValues)
            .ToListAsync();
    }

    public async Task AddAsync(RoomBotParameterValue entity)
    {
        await _dbContext.Set<RoomBotParameterValue>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(RoomBotParameterValue entity)
    {
        _dbContext.Set<RoomBotParameterValue>().Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tuple<string, string> key)
    {
        var parameterValue = await GetByIdAsync(key);
        _dbContext.Set<RoomBotParameterValue>().Remove(parameterValue);
        await _dbContext.SaveChangesAsync();
    }
}