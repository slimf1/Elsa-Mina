using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class RoomSpecificUserDataRepository
{
    private readonly DbContext _dbContext;

    public RoomSpecificUserDataRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RoomSpecificUserData> GetByIdAsync(string id)
    {
        return await _dbContext.Set<RoomSpecificUserData>()
            .Include(x => x.Badges)
            .FirstOrDefaultAsync(x => x.Id == id);
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

    public async Task DeleteAsync(string id)
    {
        var roomSpecificUserData = await GetByIdAsync(id);
        _dbContext.Set<RoomSpecificUserData>().Remove(roomSpecificUserData);
        await _dbContext.SaveChangesAsync();
    }
}