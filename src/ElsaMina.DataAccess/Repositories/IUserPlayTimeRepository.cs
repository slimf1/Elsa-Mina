using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface IUserPlayTimeRepository : IRepository<UserPlayTime, Tuple<string, string>>;