using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class AddedCommandRepository : IAddedCommandRepository
{
    private readonly BotDbContext _dbContext;
    private bool _disposed;

    public AddedCommandRepository() : this(new BotDbContext())
    {
        
    }
    
    public AddedCommandRepository(BotDbContext context)
    {
        _dbContext = context;
    }
    
    public async Task<AddedCommand> GetByIdAsync(Tuple<string, string> key)
    {
        var (commandId, roomId) = key;
        return await _dbContext.Set<AddedCommand>().FirstOrDefaultAsync(x => x.Id == commandId && x.RoomId == roomId);
    }

    public async Task<IEnumerable<AddedCommand>> GetAllAsync()
    {
        return await _dbContext.Set<AddedCommand>().ToListAsync();
    }

    public async Task AddAsync(AddedCommand addedCommand)
    {
        await _dbContext.Set<AddedCommand>().AddAsync(addedCommand);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(AddedCommand addedCommand)
    {
        _dbContext.Set<AddedCommand>().Update(addedCommand);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tuple<string, string> key)
    {
        var addedCommand = await GetByIdAsync(key);
        _dbContext.Set<AddedCommand>().Remove(addedCommand);
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
        _dbContext.Dispose();
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~AddedCommandRepository()
    {
        Dispose(false);
    }
}