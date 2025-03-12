using System.Globalization;
using ElsaMina.Core.Models;

namespace ElsaMina.Core.Contexts;

public interface IContextProvider
{
    IEnumerable<string> CurrentWhitelist { get; }
    string DefaultRoom { get; }
    CultureInfo DefaultCulture { get; }
    string GetString(string key, CultureInfo culture);
    IRoom GetRoom(string roomId);
    string GetRoomParameterValue(string roomId, string key);
}