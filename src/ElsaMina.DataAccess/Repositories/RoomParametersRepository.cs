using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class RoomParametersRepository : IRoomParametersRepository
{
    private readonly DbContext _dbContext;
    private bool _disposed;

    public RoomParametersRepository() : this(new BotDbContext())
    {
        
    }
    
    public RoomParametersRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RoomParameters> GetByIdAsync(string key)
    {
        return await _dbContext.Set<RoomParameters>()
            .FirstOrDefaultAsync(x => x.Id == key);
    }

    public async Task<IEnumerable<RoomParameters>> GetAllAsync()
    {
        return await _dbContext.Set<RoomParameters>().ToListAsync();
    }

    public async Task AddAsync(RoomParameters roomParameters)
    {
        await _dbContext.Set<RoomParameters>().AddAsync(roomParameters);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(RoomParameters roomParameters)
    {
        _dbContext.Set<RoomParameters>().Update(roomParameters);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(string key)
    {
        var roomParameters = await GetByIdAsync(key);
        _dbContext.Set<RoomParameters>().Remove(roomParameters);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
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

    ~RoomParametersRepository()
    {
        Dispose(false);
    }
}