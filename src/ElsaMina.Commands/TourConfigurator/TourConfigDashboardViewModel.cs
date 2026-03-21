using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.TourConfigurator;

public class TourConfigDashboardViewModel : LocalizableViewModel
{
    public required string BotName { get; init; }
    public required string Trigger { get; init; }
    public required string RoomId { get; init; }
    public required IReadOnlyList<DataAccess.Models.TourConfig> TourConfigs { get; init; }
}
