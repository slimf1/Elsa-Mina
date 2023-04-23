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
}