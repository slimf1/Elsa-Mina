using ElsaMina.Core.Services.Templates;

namespace ElsaMina.Commands.Arcade.Slots;

public class SlotsFunViewModel : LocalizableViewModel
{
    public required string UserName { get; init; }
    public required string UserColor { get; init; }
    public required string OutcomeMessage { get; init; }
    public required string SlotImageOne { get; init; }
    public required string SlotImageTwo { get; init; }
    public required string SlotImageThree { get; init; }
}
