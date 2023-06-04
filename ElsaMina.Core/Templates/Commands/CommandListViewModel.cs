using ElsaMina.Core.Commands;

namespace ElsaMina.Core.Templates.Commands;

public class CommandListViewModel : LocalizableViewModel
{
    public IEnumerable<ICommand> Commands { get; set; }
}