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

    private readonly Dictionary<Tuple<string, string>, string> _joinPhrases = new();

    public RoomUserDataService(IRoomSpecificUserDataRepository roomSpecificUserDataRepository,
        IBadgeHoldingRepository badgeHoldingRepository)
    {
        _roomSpecificUserDataRepository = roomSpecificUserDataRepository;
        _badgeHoldingRepository = badgeHoldingRepository;
    }

    public IReadOnlyDictionary<Tuple<string, string>, string> JoinPhrases => _joinPhrases;

    public async Task<RoomUser> GetUserData(string roomId, string userId,
        CancellationToken cancellationToken = default)
    {
        return await GetUserAndCreateIfDoesntExistAsync(roomId, userId, cancellationToken);
    }

    public async Task InitializeJoinPhrasesAsync(CancellationToken cancellationToken = default)
    {
        var fullUserData = await _roomSpecificUserDataRepository.GetAllAsync(cancellationToken);
        foreach (var userData in fullUserData)
        {
            if (string.IsNullOrEmpty(userData.JoinPhrase))
            {
                continue;
            }

            _joinPhrases[Tuple.Create(userData.Id, userData.RoomId)] = userData.JoinPhrase;
        }
    }

    private async Task<RoomUser> GetUserAndCreateIfDoesntExistAsync(string roomId, string userId,
        CancellationToken cancellationToken = default)
    {
        var existingUserData = await _roomSpecificUserDataRepository
            .GetByIdAsync(new Tuple<string, string>(userId, roomId), cancellationToken);
        if (existingUserData != null)
        {
            return existingUserData;
        }

        var userData = new RoomUser
        {
            Id = userId,
            RoomId = roomId
        };
        await _roomSpecificUserDataRepository.AddAsync(userData, cancellationToken);
        return userData;
    }

    public async Task GiveBadgeToUserAsync(string roomId, string userId, string badgeId, CancellationToken cancellationToken = default)
    {
        await GetUserAndCreateIfDoesntExistAsync(roomId, userId, cancellationToken);
        await _badgeHoldingRepository.AddAsync(new BadgeHolding
        {
            BadgeId = badgeId,
            RoomId = roomId,
            UserId = userId
        }, cancellationToken);
        await _badgeHoldingRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task TakeBadgeFromUserAsync(string roomId, string userId, string badgeId, CancellationToken cancellationToken = default)
    {
        var key = Tuple.Create(badgeId, userId, roomId);
        var badgeHolding = await _badgeHoldingRepository.GetByIdAsync(key, cancellationToken);
        if (badgeHolding == null)
        {
            throw new ArgumentException("Badge not found");
        }

        await _badgeHoldingRepository.DeleteAsync(badgeHolding, cancellationToken);
        await _badgeHoldingRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task SetUserTitleAsync(string roomId, string userId, string title,
        CancellationToken cancellationToken = default)
    {
        if (title != null && title.Length > TITLE_MAX_LENGTH)
        {
            throw new ArgumentException("Title too long");
        }

        var userData = await GetUserAndCreateIfDoesntExistAsync(roomId, userId, cancellationToken);
        userData.Title = title;
        await _roomSpecificUserDataRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task SetUserAvatarAsync(string roomId, string userId, string avatar, CancellationToken cancellationToken = default)
    {
        if (avatar != null && !avatar.IsValidImageLink())
        {
            throw new ArgumentException("Invalid URL");
        }

        var userData = await GetUserAndCreateIfDoesntExistAsync(roomId, userId, cancellationToken);
        userData.Avatar = avatar;
        await _roomSpecificUserDataRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task SetUserJoinPhraseAsync(string roomId, string userId, string joinPhrase, CancellationToken cancellationToken = default)
    {
        if (joinPhrase != null && joinPhrase.Length > JOIN_PHRASE_MAX_LENGTH)
        {
            throw new ArgumentException("Join phrase too long");
        }

        var userData = await GetUserAndCreateIfDoesntExistAsync(roomId, userId, cancellationToken);
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

        await _roomSpecificUserDataRepository.SaveChangesAsync(cancellationToken);
    }
}