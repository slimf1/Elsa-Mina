using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.TourConfigurator;

public class TourConfigFormViewModel : LocalizableViewModel
{
    public required string BotName { get; init; }
    public required string Trigger { get; init; }
    public required string RoomId { get; init; }
    public DataAccess.Models.TourConfig ExistingConfig { get; init; }
}
