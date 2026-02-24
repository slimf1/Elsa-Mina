using System.Globalization;
using ElsaMina.Core.Services.Rooms.Parameters;

namespace ElsaMina.Core.Services.Rooms;

public interface IRoom
{
    string RoomId { get; }
    string Name { get; }
    IReadOnlyDictionary<string, IUser> Users { get; }
    CultureInfo Culture { get; set; }
    TimeZoneInfo TimeZone { get; set; }
    IGame Game { get; set; }
    IEnumerable<Tuple<string, string>> LastMessages { get; }
    IDictionary<string, TimeSpan> PendingPlayTimeUpdates { get; }

    Task<string> GetParameterValueAsync(Parameter parameter, CancellationToken cancellationToken = default);
    Task<bool> SetParameterValueAsync(Parameter parameter, string value, CancellationToken cancellationToken = default);
    void AddUser(string username);
    void RemoveUser(string username);
    void RenameUser(string oldName, string newName);
    void UpdateMessageQueue(string user, string message);
}
