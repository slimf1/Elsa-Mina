using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.Badges.BadgeList;

public class BadgeListViewModel : LocalizableViewModel
{
    public Badge[] Badges { get; set; }
}