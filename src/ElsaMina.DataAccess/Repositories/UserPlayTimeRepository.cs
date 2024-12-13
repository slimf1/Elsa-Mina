using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class UserPlayTimeRepository : IUserPlayTimeRepository
{
    private readonly DbContext _dbContext;
    private bool _disposed;

    public UserPlayTimeRepository() : this(new BotDbContext())
    {
        
    }
    
    public UserPlayTimeRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserPlayTime> GetByIdAsync(Tuple<string, string> key)
    {
        var (userId, roomId) = key;
        return await _dbContext.Set<UserPlayTime>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.RoomId == roomId);
    }

    public async Task<IEnumerable<UserPlayTime>> GetAllAsync()
    {
        return await _dbContext.Set<UserPlayTime>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(UserPlayTime entity)
    {
        await _dbContext.Set<UserPlayTime>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserPlayTime entity)
    {
        _dbContext.Set<UserPlayTime>().Update(entity);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(entity).State = EntityState.Detached;
    }

    public async Task DeleteAsync(Tuple<string, string> key)
    {
        var entity = await GetByIdAsync(key);
        _dbContext.Set<UserPlayTime>().Remove(entity);
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

    ~UserPlayTimeRepository()
    {
        Dispose(false);
    }
}