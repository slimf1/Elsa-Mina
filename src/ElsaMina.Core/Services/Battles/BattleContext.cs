namespace ElsaMina.Core.Services.Battles;

public class BattleContext
{
    public BattleContext(string roomId)
    {
        RoomId = roomId;
    }

    public string RoomId { get; }
    public bool TeamPreview { get; set; }
    public bool IsBattleOver { get; set; }
    public bool HasAnnouncedStart { get; set; }
    public bool HasAnnouncedEnd { get; set; }
    public List<bool> ForceSwitchSlots { get; set; } = [];
    public List<BattlePokemonState> SidePokemon { get; set; } = [];
    public List<BattleActiveSlot> ActiveSlots { get; set; } = [];
}
