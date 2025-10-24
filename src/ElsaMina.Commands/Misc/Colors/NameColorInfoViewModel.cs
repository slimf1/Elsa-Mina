using System.Drawing;
using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Misc.Colors;

public class NameColorInfoViewModel : LocalizableViewModel
{
    public string Name { get; init; }
    public Color Color { get; init; }
    public Color? OriginalColor { get; init; }
}