using ElsaMina.Core.Services.Images;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Core.Services.RoomUserData;

public class RoomUserDataService : IRoomUserDataService
{
    private const int TITLE_MAX_LENGTH = 450;
    private const int JOIN_PHRASE_MAX_LENGTH = 300;

    private readonly IRoomSpecificUserDataRepository _roomSpecificUserDataRepository;
    private readonly IBadgeHoldingRepository _badgeHoldingRepository;
    private readonly IImageService _imageService;

    private readonly Dictionary<Tuple<string, string>, string> _joinPhrases = new();

    public RoomUserDataService(IRoomSpecificUserDataRepository roomSpecificUserDataRepository,
        IBadgeHoldingRepository badgeHoldingRepository,
        IImageService imageService)
    {
        _roomSpecificUserDataRepository = roomSpecificUserDataRepository;
        _badgeHoldingRepository = badgeHoldingRepository;
        _imageService = imageService;
    }

    public IReadOnlyDictionary<Tuple<string, string>, string> JoinPhrases => _joinPhrases;

    public async Task<RoomSpecificUserData> GetUserData(string roomId, string userId)
    {
        return await GetUserAndCreateIfDoesntExist(roomId, userId);
    }

    public async Task InitializeJoinPhrasesAsync()
    {
        var fullUserData = await _roomSpecificUserDataRepository.GetAllAsync();
        foreach (var userData in fullUserData)
        {
            if (string.IsNullOrEmpty(userData.JoinPhrase))
            {
                continue;
            }

            _joinPhrases[Tuple.Create(userData.Id, userData.RoomId)] = userData.JoinPhrase;
        }
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
        var key = Tuple.Create(badgeId, userId, roomId);
        if (await _badgeHoldingRepository.GetByIdAsync(key) == null)
        {
            throw new ArgumentException("Badge not found");
        }

        await _badgeHoldingRepository.DeleteByIdAsync(key);
    }

    public async Task SetUserTitle(string roomId, string userId, string title)
    {
        if (title != null && title.Length > TITLE_MAX_LENGTH)
        {
            throw new ArgumentException("Title too long");
        }

        var userData = await GetUserAndCreateIfDoesntExist(roomId, userId);
        userData.Title = title;
        await _roomSpecificUserDataRepository.UpdateAsync(userData);
    }

    public async Task SetUserAvatar(string roomId, string userId, string avatar)
    {
        if (avatar != null && !_imageService.IsImageLink(avatar))
        {
            throw new ArgumentException("Invalid URL");
        }

        var userData = await GetUserAndCreateIfDoesntExist(roomId, userId);
        userData.Avatar = avatar;
        await _roomSpecificUserDataRepository.UpdateAsync(userData);
    }

    public async Task SetUserJoinPhrase(string roomId, string userId, string joinPhrase)
    {
        if (joinPhrase != null && joinPhrase.Length > JOIN_PHRASE_MAX_LENGTH)
        {
            throw new ArgumentException("Join phrase too long");
        }

        var userData = await GetUserAndCreateIfDoesntExist(roomId, userId);
        userData.JoinPhrase = joinPhrase;
        var key = Tuple.Create(userData.Id, userData.RoomId);

        if (string.IsNullOrEmpty(userData.JoinPhrase))
        {
            _joinPhrases.Remove(key);
        }
        else
        {
            _joinPhrases[key] = userData.JoinPhrase;
        }

        await _roomSpecificUserDataRepository.UpdateAsync(userData);
    }
}