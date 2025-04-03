namespace ElsaMina.Core.Services.Rooms;

public interface IUser
{
    string UserId { get; }
    string Name { get; }
    bool IsIdle { get; }
    Rank Rank { get; }
}