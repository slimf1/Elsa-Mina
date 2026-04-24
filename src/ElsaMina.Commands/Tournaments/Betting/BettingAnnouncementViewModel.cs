using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Tournaments.Betting;

public class BettingAnnouncementViewModel : LocalizableViewModel
{
    public string[] Players { get; init; }
    public string BotName { get; init; }
    public string Trigger { get; init; }
    public string RoomId { get; init; }
    public int SecondsToClose { get; init; }
}