using ElsaMina.Core.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Badges.BadgeHolders;

public class BadgeHoldersViewModel : LocalizableViewModel
{
    public Badge[] Badges { get; set; }
}
