using System.Drawing;
using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Misc.Colors;

public class NameColorInfoViewModel : LocalizableViewModel
{
    public string Name { get; set; }
    public Color Color { get; set; }
}