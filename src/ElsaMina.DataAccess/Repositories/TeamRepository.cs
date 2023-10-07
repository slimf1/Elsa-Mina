using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly DbContext _dbContext;
    private bool _disposed;

    public TeamRepository() : this(new BotDbContext())
    {
        
    }

    public TeamRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Team> GetByIdAsync(string key)
    {
        return await _dbContext.Set<Team>()
            .Include(x => x.Rooms)
            .ThenInclude(x => x.RoomParameters)
            .FirstOrDefaultAsync(x => x.Id == key);
    }

    public async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _dbContext.Set<Team>()
            .Include(x => x.Rooms)
            .ThenInclude(x => x.RoomParameters)
            .ToListAsync();
    }

    public async Task AddAsync(Team entity)
    {
        await _dbContext.Set<Team>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Team entity)
    {
        _dbContext.Set<Team>().Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(string key)
    {
        var team = await GetByIdAsync(key);
        _dbContext.Set<Team>().Remove(team);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Team>> GetTeamsFromRoom(string roomId)
    {
        return await _dbContext.Set<Team>()
            .Include(x => x.Rooms)
            .ThenInclude(x => x.RoomParameters)
            .Where(x => x.Rooms.Any(room => room.RoomId == roomId))
            .ToListAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed || !disposing)
        {
            return;
        }
        
        _dbContext?.Dispose();
        _disposed = true;
    }

    ~TeamRepository()
    {
        Dispose(false);
    }
}