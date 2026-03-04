using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess.Models;

namespace ElsaMina.Commands.CustomCommands;

public class CustomCommandListViewModel : LocalizableViewModel
{
    public IEnumerable<AddedCommand> Commands { get; init; }
}