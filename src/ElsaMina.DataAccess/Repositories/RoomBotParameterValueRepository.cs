using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.DataAccess.Repositories;

public class RoomBotParameterValueRepository : BaseRepository<RoomBotParameterValue, Tuple<string, string>>, IRoomBotParameterValueRepository
{
    private readonly DbContext _dbContext;
    
    public RoomBotParameterValueRepository(DbContext context) : base(context)
    {
        _dbContext = context;
    }

    public override async Task<RoomBotParameterValue> GetByIdAsync(Tuple<string, string> key)
    {
        var (roomId, parameterId) = key;
        return await _dbContext.Set<RoomBotParameterValue>()
            .AsNoTracking()
            .Include(x => x.RoomParameters)
            .ThenInclude(x => x.ParameterValues)
            .FirstOrDefaultAsync(x => x.RoomId == roomId && x.ParameterId == parameterId);
    }

    public override async Task<IEnumerable<RoomBotParameterValue>> GetAllAsync()
    {
        return await _dbContext.Set<RoomBotParameterValue>()
            .AsNoTracking()
            .Include(x => x.RoomParameters)
            .ThenInclude(x => x.ParameterValues)
            .ToListAsync();
    }
}