using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Users.PlayTimes;

public class TopPlayTimesViewModel : LocalizableViewModel
{
    public string Room { get; init; }
    public IEnumerable<TopPlayTimesEntry> TopList { get; init; }
}
