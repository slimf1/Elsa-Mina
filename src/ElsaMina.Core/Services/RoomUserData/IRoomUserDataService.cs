using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Services.RoomUserData;

public interface IRoomUserDataService
{
    IReadOnlyDictionary<Tuple<string, string>, string> JoinPhrases { get; }
    Task InitializeJoinPhrasesAsync(CancellationToken cancellationToken = default);
    Task<RoomSpecificUserData> GetUserData(string roomId, string userId);
    Task GiveBadgeToUser(string roomId, string userId, string badgeId);
    Task TakeBadgeFromUser(string roomId, string userId, string badgeId);
    Task SetUserTitle(string roomId, string userId, string title);
    Task SetUserAvatar(string roomId, string userId, string avatar);
    Task SetUserJoinPhrase(string roomId, string userId, string joinPhrase);
}