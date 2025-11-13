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

    public override async Task<PollSuggestion> GetByIdAsync(int key, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<PollSuggestion>()
            .FirstOrDefaultAsync(x => x.Id == key, cancellationToken: cancellationToken);
    }

    public override async Task<IEnumerable<PollSuggestion>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<PollSuggestion>()
            .ToListAsync(cancellationToken: cancellationToken);
    }
}