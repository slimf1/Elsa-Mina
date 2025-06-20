﻿using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.UnitTests.Fixtures;

public static class UserFixtures
{
    public static IUser VoicedUser(string name = "testUser") => RankedUser(name, Rank.Voiced);

    public static IUser DriverUser(string name = "testUser") => RankedUser(name, Rank.Driver);

    public static IUser ModUser(string name = "testUser") => RankedUser(name, Rank.Mod);

    public static IUser RoomOwnerUser(string name = "testUser") => RankedUser(name, Rank.RoomOwner);

    public static IUser AdminUser(string name = "testUser") => RankedUser(name, Rank.Admin);

    private static User RankedUser(string name, Rank rank) => new User(name, rank);
}