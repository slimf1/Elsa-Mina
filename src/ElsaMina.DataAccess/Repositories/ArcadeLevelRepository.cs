using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class ArcadeLevelRepository : BaseRepository<ArcadeLevel, string>, IArcadeLevelRepository
{
    private readonly DbContext _dbContext;

    public ArcadeLevelRepository(DbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<ArcadeLevel> GetByIdAsync(string key, CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<ArcadeLevel>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == key, cancellationToken: cancellationToken);
    }

    public override async Task<IEnumerable<ArcadeLevel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ArcadeLevel>()
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);
    }
}