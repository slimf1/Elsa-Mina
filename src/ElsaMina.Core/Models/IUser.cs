namespace ElsaMina.Core.Models;

public interface IUser
{
    string UserId { get; }
    string Name { get; }
    bool IsIdle { get; }
    Rank Rank { get; }
}