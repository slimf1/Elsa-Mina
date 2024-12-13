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

    public override async Task<RoomParameters> GetByIdAsync(string key)
    {
        return await _dbContext.Set<RoomParameters>()
            .AsNoTracking()
            .Include(x => x.Teams)
            .Include(x => x.ParameterValues)
            .FirstOrDefaultAsync(x => x.Id == key);
    }

    public override async Task<IEnumerable<RoomParameters>> GetAllAsync()
    {
        return await _dbContext.Set<RoomParameters>()
            .AsNoTracking()
            .ToListAsync();
    }
}