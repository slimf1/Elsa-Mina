using ElsaMina.DataAccess.Models;

namespace ElsaMina.Core.Services.RoomUserData;

public interface IRoomUserDataService
{
    Task<RoomSpecificUserData> GetUserData(string roomId, string userId);
    Task GiveBadgeToUser(string roomId, string userId, string badgeId);
}