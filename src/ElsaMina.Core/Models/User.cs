using ElsaMina.Core.Utils;

namespace ElsaMina.Core.Models;

public sealed class User : IUser, IEquatable<User>
{
    private static readonly IReadOnlyDictionary<char, Rank> RANK_MAPPING = new Dictionary<char, Rank>
    {
        [' '] = Rank.Regular,
        ['+'] = Rank.Voiced,
        ['%'] = Rank.Driver,
        ['@'] = Rank.Mod,
        ['*'] = Rank.Bot,
        ['#'] = Rank.RoomOwner,
        ['&'] = Rank.Leader,
        ['~'] = Rank.Admin
    };
    
    public static IUser FromUsername(string username)
    {
        var rank = RANK_MAPPING.ContainsKey(username[0]) ? RANK_MAPPING[username[0]] : Rank.Regular;
        return new User(username[1..], rank);
    }
    
    public User(string name, Rank rank)
    {
        UserId = name.ToLowerAlphaNum();
        IsIdle = name.Length >= 2 && name[^2..] == "@!";
        Name = IsIdle ? name[..^2] : name;
        Rank = rank;
    }
    
    public string UserId { get; }
    public string Name { get; }
    public bool IsIdle { get; }
    public Rank Rank { get; }

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
        return UserId != null ? UserId.GetHashCode() : 0;
    }
}