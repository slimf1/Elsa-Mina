using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class RoomSpecificUserDataRepository : IRepository<RoomSpecificUserData, Tuple<string, string>>
{
    private readonly DbContext _dbContext;
    private bool _disposed;

    public RoomSpecificUserDataRepository() : this(new BotDbContext())
    {
        
    }
    
    public RoomSpecificUserDataRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RoomSpecificUserData> GetByIdAsync(Tuple<string, string> key)
    {
        var (userId, roomId) = key;
        return await _dbContext.Set<RoomSpecificUserData>()
            .Include(x => x.Badges)
            .FirstOrDefaultAsync(x => x.Id == userId && x.RoomId == roomId);
    }

    public async Task<IEnumerable<RoomSpecificUserData>> GetAllAsync()
    {
        return await _dbContext.Set<RoomSpecificUserData>()
            .Include(x => x.Badges)
            .ToListAsync();
    }

    public async Task AddAsync(RoomSpecificUserData roomSpecificUserData)
    {
        await _dbContext.Set<RoomSpecificUserData>().AddAsync(roomSpecificUserData);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(RoomSpecificUserData roomSpecificUserData)
    {
        _dbContext.Set<RoomSpecificUserData>().Update(roomSpecificUserData);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tuple<string, string> key)
    {
        var roomSpecificUserData = await GetByIdAsync(key);
        _dbContext.Set<RoomSpecificUserData>().Remove(roomSpecificUserData);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~RoomSpecificUserDataRepository()
    {
        Dispose(false);
    }
}