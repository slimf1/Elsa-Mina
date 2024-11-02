using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.CustomColors;
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

    public override async Task Run(IContext context)
    {
        if (string.IsNullOrEmpty(context.Target))
        {
            ReplyLocalizedHelpMessage(context);
            return;
        }
        
        var color = _customColorsManager.CustomColorsMapping.TryGetValue(context.Target, out var customColorName)
            ? customColorName.ToColor()
            : context.Target.ToColor();
        var template = await _templatesManager.GetTemplate("Misc/Colors/NameColorInfo", new NameColorInfoViewModel
        {
            Name = context.Target,
            Color = color
        });
        context.SendHtml(template.RemoveNewlines(), rankAware: true);
    }
}