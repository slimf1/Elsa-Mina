using ElsaMina.Core.Contexts;

namespace ElsaMina.Core.Services.Repeats;

public interface IRepeatsManager
{
    void StartRepeat(IContext context, string roomId, string message, TimeSpan interval);
    IRepeat GetRepeat(Guid repeatId);
    IEnumerable<IRepeat> GetRepeats(string roomId);
    bool StopRepeat(Guid repeatId);
}