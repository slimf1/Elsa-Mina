using ElsaMina.Core.Commands;

namespace ElsaMina.Core.Templates.Commands;

public class CommandListViewModel : BaseViewModel
{
    public IEnumerable<ICommand> Commands { get; set; }
}