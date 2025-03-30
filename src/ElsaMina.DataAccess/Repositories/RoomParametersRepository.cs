using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class RoomParametersRepository : BaseRepository<RoomParameters, string>, IRoomParametersRepository
{
    private readonly DbContext _dbContext;
    
    public RoomParametersRepository(DbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<RoomParameters> GetByIdAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<RoomParameters>()
            .AsNoTracking()
            .Include(x => x.Teams)
            .Include(x => x.ParameterValues)
            .FirstOrDefaultAsync(x => x.Id == key, cancellationToken: cancellationToken);
    }

    public override async Task<IEnumerable<RoomParameters>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<RoomParameters>()
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);
    }
}