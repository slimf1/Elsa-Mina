using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Games.VoltorbFlip;

public class VoltorbFlipModel : LocalizableViewModel
{
    public required IVoltorbFlipGame CurrentGame { get; init; }
    public required string Trigger { get; init; }
    public required string BotName { get; init; }
    public required string RoomId { get; init; }
    public bool ShowAll { get; init; }
    public bool IsPrivateMode { get; init; }
}
