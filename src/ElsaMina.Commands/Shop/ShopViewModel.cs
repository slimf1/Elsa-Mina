using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Shop;

public class ShopViewModel : LocalizableViewModel
{
    public required Dictionary<string, List<ShopItem>> Items { get; init; }
    public required string BotName { get; init; }
}
