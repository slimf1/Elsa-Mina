using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface IBadgeRepository : IRepository<Badge, Tuple<string, string>>;