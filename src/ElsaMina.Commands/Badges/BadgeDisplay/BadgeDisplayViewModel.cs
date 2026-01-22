using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Badges.BadgeDisplay;

public class BadgeDisplayViewModel : LocalizableViewModel
{
    public Badge DisplayedBadge { get; set; }
    public string[] BadgeHolders { get; set; }
}