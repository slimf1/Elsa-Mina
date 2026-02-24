using System.Globalization;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Core.Contexts;

public interface IContextProvider
{
    string DefaultRoom { get; }
    string BugReportLink { get; }
    CultureInfo DefaultCulture { get; }
    bool IsUserWhitelisted(string userId);
    string GetString(string key, CultureInfo culture);
    IRoom GetRoom(string roomId);
    Task<Rank> GetUserRankInRoom(string roomId, string userId, CancellationToken cancellationToken);
}
