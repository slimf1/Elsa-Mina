using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Models;

public class User : IUser, IEquatable<User>
{
    public User(string name, char rank)
    {
        UserId = name.ToLowerAlphaNum();
        IsIdle = name.Length >= 2 && name[^2..] == "@!";
        Name = IsIdle ? name[..^2] : name;
        Rank = rank;
    }
    
    public string UserId { get; }
    public string Name { get; }
    public bool IsIdle { get; }
    public char Rank { get; }

    public bool Equals(User other)
    {
        return UserId == other?.UserId;
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((User)obj);
    }

    public override int GetHashCode()
    {
        return (UserId != null ? UserId.GetHashCode() : 0);
    }

    public override string ToString()
    {
        return $"{nameof(User)}[{nameof(UserId)}: {UserId}, " +
               $"{nameof(Name)}: {Name}, " +
               $"{nameof(IsIdle)}: {IsIdle}, " +
               $"{nameof(Rank)}: {Rank}]";
    }
}