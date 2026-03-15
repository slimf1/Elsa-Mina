using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Shop;

[NamedCommand("editshop")]
public class EditShopCommand : Command
{
    private readonly IShopService _shopService;
    private readonly ITemplatesManager _templatesManager;
    private readonly IConfiguration _configuration;

    public EditShopCommand(IShopService shopService, ITemplatesManager templatesManager,
        IConfiguration configuration)
    {
        _shopService = shopService;
        _templatesManager = templatesManager;
        _configuration = configuration;
    }

    public override Rank RequiredRank => Rank.Voiced;
    public override bool IsAllowedInPrivateMessage => true;
    public override bool IsWhitelistOnly => true;
    public override string HelpMessageKey => "shop_edit_help";

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var shopData = await _shopService.GetShopDataAsync();
        var template = await _templatesManager.GetTemplateAsync("Shop/ShopEditPanel", new ShopViewModel
        {
            Culture = context.Culture,
            Items = shopData,
            BotName = _configuration.Name
        });
        context.ReplyHtmlPage("shop-edit", template.RemoveNewlines().CollapseAttributeWhitespace());
    }
}
