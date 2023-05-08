using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class RoomParametersRepository : IRoomParametersRepository
{
    private readonly DbContext _dbContext;

    public RoomParametersRepository() : this(new BotDbContext())
    {
        
    }
    
    public RoomParametersRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RoomParameters> GetByIdAsync(string id)
    {
        return await _dbContext.Set<RoomParameters>()
            .FirstOrDefaultAsync(x => x.Id == id);
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

    public async Task DeleteAsync(string id)
    {
        var roomParameters = await GetByIdAsync(id);
        _dbContext.Set<RoomParameters>().Remove(roomParameters);
        await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}