using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Shop;

[NamedCommand("edititem")]
public class EditItemCommand : Command
{
    private readonly IShopService _shopService;

    public EditItemCommand(IShopService shopService)
    {
        _shopService = shopService;
    }

    public override Rank RequiredRank => Rank.Voiced;
    public override bool IsAllowedInPrivateMessage => true;
    public override bool IsWhitelistOnly => true;
    public override string HelpMessageKey => "shop_edit_item_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var args = context.Target.Split(',', 4);
        if (args.Length != 4)
        {
            context.ReplyLocalizedMessage(HelpMessageKey);
            return;
        }

        var rawId = args[0].Trim();
        var newArticle = args[2].Trim();
        var newPrice = args[3].Trim();

        if (!int.TryParse(rawId, out var itemId))
        {
            context.ReplyLocalizedMessage("shop_invalid_id");
            return;
        }

        var updated = await _shopService.UpdateItemAsync(itemId, newArticle, newPrice, cancellationToken);
        if (updated == null)
        {
            context.ReplyLocalizedMessage("shop_item_not_found", itemId);
            return;
        }

        context.ReplyLocalizedMessage("shop_item_edited", itemId);
    }
}
