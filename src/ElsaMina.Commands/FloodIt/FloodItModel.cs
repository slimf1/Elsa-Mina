using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.FloodIt;

public class FloodItModel : LocalizableViewModel
{
    public required IFloodItGame CurrentGame { get; init; }
    public required string Trigger { get; init; }
    public required string BotName { get; init; }
    public required string RoomId { get; init; }
    public bool IsPrivateMode { get; init; }
}
