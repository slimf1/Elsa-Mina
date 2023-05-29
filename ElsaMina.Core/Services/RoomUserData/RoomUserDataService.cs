using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Services.RoomUserData;

public class RoomUserDataService : IRoomUserDataService
{
    private readonly IRepository<RoomSpecificUserData, Tuple<string, string>> _roomSpecificUserDataRepository;
    private readonly IRepository<Badge, Tuple<string, string>> _badgeRepository;

    public RoomUserDataService(IRepository<RoomSpecificUserData, Tuple<string, string>> roomSpecificUserDataRepository,
        IRepository<Badge, Tuple<string, string>> badgeRepository)
    {
        _roomSpecificUserDataRepository = roomSpecificUserDataRepository;
        _badgeRepository = badgeRepository;
    }

    public async Task<RoomSpecificUserData> GetUserData(string roomId, string userId)
    {
        var userData = await _roomSpecificUserDataRepository.GetByIdAsync(new(userId, roomId));
        if (userData != null)
        {
            return userData;
        }

        userData = new RoomSpecificUserData
        {
            Id = userId,
            RoomId = roomId
        };
        await _roomSpecificUserDataRepository.AddAsync(userData);

        return userData;
    }

    public async Task GiveBadgeToUser(string roomId, string userId, Badge badge)
    {
        var userData = await GetUserData(roomId, userId);
        userData.Badges.Add(badge);
        await _roomSpecificUserDataRepository.UpdateAsync(userData);
    }
}