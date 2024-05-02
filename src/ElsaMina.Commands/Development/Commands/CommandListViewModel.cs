using ElsaMina.Core.Commands;
using ElsaMina.Core.Templates;

namespace ElsaMina.Commands.Development.Commands;

public class CommandListViewModel : LocalizableViewModel
{
    public IEnumerable<ICommand> Commands { get; set; }
}