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

    public override bool Equals(object other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other.GetType() != GetType()) return false;
        return Equals((User)other);
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