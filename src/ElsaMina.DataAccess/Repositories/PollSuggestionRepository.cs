using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class PollSuggestionRepository : IPollSuggestionRepository
{
    private readonly DbContext _dbContext;
    private bool _disposed;

    public PollSuggestionRepository() : this(new BotDbContext())
    {
    }

    public PollSuggestionRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PollSuggestion> GetByIdAsync(int key)
    {
        return await _dbContext.Set<PollSuggestion>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == key);
    }

    public async Task<IEnumerable<PollSuggestion>> GetAllAsync()
    {
        return await _dbContext.Set<PollSuggestion>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(PollSuggestion entity)
    {
        await _dbContext.Set<PollSuggestion>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(PollSuggestion entity)
    {
        _dbContext.Set<PollSuggestion>().Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int key)
    {
        var entity = await GetByIdAsync(key);
        _dbContext.Set<PollSuggestion>().Remove(entity);
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

    ~PollSuggestionRepository()
    {
        Dispose(false);
    }
}