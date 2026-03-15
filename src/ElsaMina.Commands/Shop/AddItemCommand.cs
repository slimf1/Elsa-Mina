using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Shop;

[NamedCommand("additem")]
public class AddItemCommand : Command
{
    private static readonly string[] VALID_PALIERS = ["1", "2", "3", "4"];

    private readonly IShopService _shopService;

    public AddItemCommand(IShopService shopService)
    {
        _shopService = shopService;
    }

    public override Rank RequiredRank => Rank.Voiced;
    public override bool IsAllowedInPrivateMessage => true;
    public override bool IsWhitelistOnly => true;
    public override string HelpMessageKey => "shop_add_item_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var args = context.Target.Split(',', 3);
        if (args.Length != 3)
        {
            context.ReplyLocalizedMessage(HelpMessageKey);
            return;
        }

        var article = args[0].Trim();
        var price = args[1].Trim();
        var palier = args[2].Trim();

        if (!VALID_PALIERS.Contains(palier))
        {
            context.ReplyLocalizedMessage("shop_invalid_palier");
            return;
        }

        var added = await _shopService.AddItemAsync(palier, article, price, cancellationToken);
        context.ReplyLocalizedMessage("shop_item_added", article, palier, price, added.Id);
    }
}
