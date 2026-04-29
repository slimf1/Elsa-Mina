using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Games.LightsOut;

public class LightsOutModel : LocalizableViewModel
{
    public required ILightsOutGame CurrentGame { get; init; }
    public required string Trigger { get; init; }
    public required string BotName { get; init; }
    public required string RoomId { get; init; }
    public bool IsPrivateMode { get; init; }
}
