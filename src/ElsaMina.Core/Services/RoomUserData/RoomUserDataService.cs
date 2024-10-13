using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Services.RoomUserData;

public class RoomUserDataService : IRoomUserDataService
{
    private const int TITLE_MAX_LENGTH = 450;
    private const int JOIN_PHRASE_MAX_LENGTH = 300;

    private readonly IRoomSpecificUserDataRepository _roomSpecificUserDataRepository;
    private readonly IBadgeHoldingRepository _badgeHoldingRepository;

    public RoomUserDataService(IRoomSpecificUserDataRepository roomSpecificUserDataRepository,
        IBadgeHoldingRepository badgeHoldingRepository)
    {
        _roomSpecificUserDataRepository = roomSpecificUserDataRepository;
        _badgeHoldingRepository = badgeHoldingRepository;
    }

    public async Task<RoomSpecificUserData> GetUserData(string roomId, string userId)
    {
        return await GetUserAndCreateIfDoesntExist(roomId, userId);
    }

    private async Task<RoomSpecificUserData> GetUserAndCreateIfDoesntExist(string roomId, string userId)
    {
        var existingUserData = await _roomSpecificUserDataRepository.GetByIdAsync(new(userId, roomId));
        if (existingUserData != null)
        {
            return existingUserData;
        }

        var userData = new RoomSpecificUserData
        {
            Id = userId,
            RoomId = roomId
        };
        await _roomSpecificUserDataRepository.AddAsync(userData);

        return userData;
    }

    public async Task GiveBadgeToUser(string roomId, string userId, string badgeId)
    {
        await GetUserAndCreateIfDoesntExist(roomId, userId);
        await _badgeHoldingRepository.AddAsync(new BadgeHolding
        {
            BadgeId = badgeId,
            RoomId = roomId,
            UserId = userId
        });
    }

    public async Task TakeBadgeFromUser(string roomId, string userId, string badgeId)
    {
        var key = new Tuple<string, string, string>(badgeId, userId, roomId);
        if (await _badgeHoldingRepository.GetByIdAsync(key) == null)
        {
            throw new ArgumentException("Badge not found");
        }

        await _badgeHoldingRepository.DeleteAsync(key);
    }

    public async Task SetUserTitle(string roomId, string userId, string title)
    {
        if (title.Length > TITLE_MAX_LENGTH)
        {
            throw new ArgumentException("Title too long");
        }

        var userData = await GetUserAndCreateIfDoesntExist(roomId, userId);
        userData.Title = title;
        await _roomSpecificUserDataRepository.UpdateAsync(userData);
    }

    public async Task SetUserAvatar(string roomId, string userId, string avatar)
    {
        if (!Images.IMAGE_LINK_REGEX.IsMatch(avatar))
        {
            throw new ArgumentException("Invalid URL");
        }

        var userData = await GetUserAndCreateIfDoesntExist(roomId, userId);
        userData.Avatar = avatar;
        await _roomSpecificUserDataRepository.UpdateAsync(userData);
    }
    
    public async Task SetUserJoinPhrase(string roomId, string userId, string joinPhrase)
    {
        if (joinPhrase.Length > JOIN_PHRASE_MAX_LENGTH)
        {
            throw new ArgumentException("Join phrase too long");
        }
        var userData = await GetUserAndCreateIfDoesntExist(roomId, userId);
        userData.JoinPhrase = joinPhrase;
        await _roomSpecificUserDataRepository.UpdateAsync(userData);
    }
}