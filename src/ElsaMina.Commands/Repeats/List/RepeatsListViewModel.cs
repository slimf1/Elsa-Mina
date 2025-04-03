using ElsaMina.Core.Services.Repeats;
using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Repeats.List;

public class RepeatsListViewModel : LocalizableViewModel
{
    public IEnumerable<IRepeat> Repeats { get; init; }
    public string BotName { get; init; }
    public string Trigger { get; init; }
}