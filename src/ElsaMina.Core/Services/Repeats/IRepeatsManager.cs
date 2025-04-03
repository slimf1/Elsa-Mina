using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.Repeats;

public interface IRepeatsManager
{
    bool StartRepeat(IContext context, string message, TimeSpan interval);
    IRepeat GetRepeat(string roomId, Guid repeatId);
    IEnumerable<IRepeat> GetRepeats(string roomId);
    bool StopRepeat(string roomId, Guid repeatId);
}