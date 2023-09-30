using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface IBadgeHoldingRepository : IRepository<BadgeHolding, Tuple<string, string, string>>
{
    
}