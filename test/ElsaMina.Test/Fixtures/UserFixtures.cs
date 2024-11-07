using ElsaMina.Core.Models;

namespace ElsaMina.Test.Fixtures;

public static class UserFixtures
{
    public static IUser VoicedUser(string name = "testUser") => RankedUser(name, '+');

    public static IUser DriverUser(string name = "testUser") => RankedUser(name, '%');

    public static IUser ModUser(string name = "testUser") => RankedUser(name, '@');

    public static IUser RoomOwnerUser(string name = "testUser") => RankedUser(name, '#');

    public static IUser AdminUser(string name = "testUser") => RankedUser(name, '&');

    private static User RankedUser(string name, char rank) => new User(name, rank);
}