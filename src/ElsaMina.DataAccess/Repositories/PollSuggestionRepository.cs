using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class PollSuggestionRepository : BaseRepository<PollSuggestion, int>, IPollSuggestionRepository
{
    private readonly DbContext _dbContext;

    public PollSuggestionRepository(DbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<PollSuggestion> GetByIdAsync(int key)
    {
        return await _dbContext.Set<PollSuggestion>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == key);
    }

    public override async Task<IEnumerable<PollSuggestion>> GetAllAsync()
    {
        return await _dbContext.Set<PollSuggestion>()
            .AsNoTracking()
            .ToListAsync();
    }
}