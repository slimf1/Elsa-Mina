using System.Drawing;
using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.CustomColors;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;

namespace ElsaMina.Commands.Misc.Colors;

[NamedCommand("namecolor", Aliases = ["name-color", "colorinfo", "namecolorinfo"])]
public class NameColorInfo : Command
{
    private readonly ITemplatesManager _templatesManager;
    private readonly ICustomColorsManager _customColorsManager;

    public NameColorInfo(ITemplatesManager templatesManager, ICustomColorsManager customColorsManager)
    {
        _templatesManager = templatesManager;
        _customColorsManager = customColorsManager;
    }

    public override bool IsAllowedInPrivateMessage => true;
    public override string HelpMessageKey => "name_color_help";
    public override Rank RequiredRank => Rank.Regular;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(context.Target))
        {
            ReplyLocalizedHelpMessage(context);
            return;
        }

        Color color = context.Target.ToColor();
        Color? originalColor = null;
        if (_customColorsManager.CustomColorsMapping.TryGetValue(context.Target, out var customColorName))
        {
            originalColor = customColorName.ToColor();
        }
        var template = await _templatesManager.GetTemplateAsync("Misc/Colors/NameColorInfo", new NameColorInfoViewModel
        {
            Culture = context.Culture,
            Name = context.Target,
            Color = color,
            OriginalColor = originalColor
        });
        context.SendHtmlIn(template.RemoveNewlines(), rankAware: true);
    }
}