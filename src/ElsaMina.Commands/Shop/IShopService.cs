using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Shop;

public interface IShopService
{
    Task<Dictionary<string, List<ShopItem>>> GetShopDataAsync();
    Task<ShopItem> AddItemAsync(string tier, string article, string price, CancellationToken cancellationToken = default);
    Task<ShopItem> UpdateItemAsync(int id, string article, string price, CancellationToken cancellationToken = default);
    Task<bool> RemoveItemAsync(string tier, string article, CancellationToken cancellationToken = default);
}
