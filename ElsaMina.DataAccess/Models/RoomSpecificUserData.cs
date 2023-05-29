﻿namespace ElsaMina.DataAccess.Models;

public class RoomSpecificUserData : IKeyed<Tuple<string, string>>
{
    public Tuple<string, string> Key => new(Id, RoomId);

    public RoomSpecificUserData()
    {
        Badges = new HashSet<Badge>();
    }

    public string Id { get; set; }
    public string RoomId { get; set; }
    public long? OnTime { get; set; }
    public string? Avatar { get; set; }
    public string? Title { get; set; }
    public ICollection<Badge> Badges { get; set; }
}