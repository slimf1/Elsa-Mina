using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ElsaMina.DataAccess.Repositories;

public class RoomSpecificUserDataRepository : IRoomSpecificUserDataRepository
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
            .ThenInclude(x => x.Badge)
            .FirstOrDefaultAsync(x => x.Id == userId && x.RoomId == roomId);
    }

    public async Task<IEnumerable<RoomSpecificUserData>> GetAllAsync()
    {
        return await _dbContext.Set<RoomSpecificUserData>()
            .Include(x => x.Badges)
            .ThenInclude(x => x.Badge)
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