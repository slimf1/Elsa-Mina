using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Models;

public class User : IUser
{
    public string UserId { get; }
    public string Name { get; }
    public bool IsIdle { get; }
    public char Rank { get; }

    public User(string name, char rank)
    {
        UserId = name.ToLowerAlphaNum();
        IsIdle = name[^2..] == "@!";
        Name = IsIdle ? name[..^2] : name;
        Rank = rank;
    }

    public override string ToString()
    {
        return $"{nameof(User)}[{nameof(UserId)}: {UserId}, " +
               $"{nameof(Name)}: {Name}, " +
               $"{nameof(IsIdle)}: {IsIdle}, " +
               $"{nameof(Rank)}: {Rank}]";
    }
}