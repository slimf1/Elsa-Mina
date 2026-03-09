using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.VoltorbFlip;

public class VoltorbFlipModel : LocalizableViewModel
{
    public required IVoltorbFlipGame CurrentGame { get; init; }
    public required string Trigger { get; init; }
    public required string BotName { get; init; }
    public required string RoomId { get; init; }
    public bool ShowAll { get; init; }
}
