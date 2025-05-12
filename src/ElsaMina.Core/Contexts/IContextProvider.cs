using System.Globalization;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Contexts;

public interface IContextProvider
{
    string DefaultRoom { get; }
    CultureInfo DefaultCulture { get; }
    bool IsUserWhitelisted(string userId);
    string GetString(string key, CultureInfo culture);
    IRoom GetRoom(string roomId);
    string GetRoomParameterValue(string roomId, string key);
    Task<Rank> GetUserRankInRoom(string roomId, string userId, CancellationToken cancellationToken);
}