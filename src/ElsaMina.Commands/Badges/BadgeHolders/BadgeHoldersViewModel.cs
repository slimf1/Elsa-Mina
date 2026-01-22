using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Badges.BadgeHolders;

public class BadgeHoldersViewModel : LocalizableViewModel
{
    public Badge[] Badges { get; set; }
}
