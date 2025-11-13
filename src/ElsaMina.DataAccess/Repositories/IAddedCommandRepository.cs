using ElsaMina.DataAccess.Models;

namespace ElsaMina.DataAccess.Repositories;

public interface IAddedCommandRepository : IRepository<AddedCommand, Tuple<string, string>>;