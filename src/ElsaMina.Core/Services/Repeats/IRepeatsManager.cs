using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.Repeats;

public interface IRepeatsManager
{
    Repeat StartRepeat(IContext context, string repeatId, string message, uint intervalInMinutes);
    Repeat GetRepeat(string roomId, string repeatId);
    bool StopRepeat(string roomId, string repeatId);
}