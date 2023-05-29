using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Services.RoomUserData;

public class RoomUserDataService : IRoomUserDataService
{
    private readonly IRepository<RoomSpecificUserData, Tuple<string, string>> _roomSpecificUserDataRepository;
    private readonly IRepository<BadgeHolding, Tuple<string, string, string>> _badgeHoldingRepository;

    public RoomUserDataService(IRepository<RoomSpecificUserData, Tuple<string, string>> roomSpecificUserDataRepository,
        IRepository<BadgeHolding, Tuple<string, string, string>> badgeHoldingRepository)
    {
        _roomSpecificUserDataRepository = roomSpecificUserDataRepository;
        _badgeHoldingRepository = badgeHoldingRepository;
    }

    public async Task<RoomSpecificUserData> GetUserData(string roomId, string userId)
    {
        await CreateUserIfDoesntExist(roomId, userId);
        return await _roomSpecificUserDataRepository.GetByIdAsync(new(userId, roomId));
    }

    private async Task CreateUserIfDoesntExist(string roomId, string userId)
    {
        if (await _roomSpecificUserDataRepository.GetByIdAsync(new(userId, roomId)) != null)
        {
            return;
        }

        var userData = new RoomSpecificUserData
        {
            Id = userId,
            RoomId = roomId
        };
        await _roomSpecificUserDataRepository.AddAsync(userData);
    }

    public async Task GiveBadgeToUser(string roomId, string userId, string badgeId)
    {
        await CreateUserIfDoesntExist(roomId, userId);
        await _badgeHoldingRepository.AddAsync(new BadgeHolding
        {
            BadgeId = badgeId,
            RoomId = roomId,
            UserId = userId
        });
        /*
        await CreateUserIfDoesntExist(roomId, userId);
        var userData = await _roomSpecificUserDataRepository.GetByIdAsync(new(userId, roomId));
        var badge = await _badgeRepository.GetByIdAsync(new(badgeId, roomId));
        userData.Badges.Add(badge);
        await _roomSpecificUserDataRepository.UpdateAsync(userData);
        */


        //var userData = await GetUserData(roomId, userId);
        //badge.BadgeHolders.Add(userData);
        //userData.Badges.Add(badge);
        //await _badgeRepository.UpdateAsync(badge);
    }
}