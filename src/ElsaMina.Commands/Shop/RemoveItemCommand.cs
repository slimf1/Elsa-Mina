using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Shop;

[NamedCommand("removeitem")]
public class RemoveItemCommand : Command
{
    private readonly IShopService _shopService;

    public RemoveItemCommand(IShopService shopService)
    {
        _shopService = shopService;
    }

    public override Rank RequiredRank => Rank.Voiced;
    public override bool IsAllowedInPrivateMessage => true;
    public override bool IsWhitelistOnly => true;
    public override string HelpMessageKey => "shop_remove_item_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var args = context.Target.Split(',', 2);
        if (args.Length != 2)
        {
            context.ReplyLocalizedMessage(HelpMessageKey);
            return;
        }

        var palier = args[0].Trim();
        var article = args[1].Trim();

        var removed = await _shopService.RemoveItemAsync(palier, article, cancellationToken);
        if (!removed)
        {
            context.ReplyLocalizedMessage("shop_item_not_in_palier", article, palier);
            return;
        }

        context.ReplyLocalizedMessage("shop_item_removed", article, palier);
    }
}
