using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.CustomCommands;

public class AddCustomCommand : ICommand
{
    public static string Name => "add-custom-command";
    public static IEnumerable<string> Aliases => new[] { "add-custom", "add-command" };
    public char RequiredRank => '@';

    private readonly IAddedCommandRepository _addedCommandRepository;
    private readonly IConfigurationManager _configurationManager;
    
    public AddCustomCommand(IAddedCommandRepository addedCommandRepository,
        IConfigurationManager configurationManager)
    {
        _addedCommandRepository = addedCommandRepository;
        _configurationManager = configurationManager;
    }

    public async Task Run(Context context)
    {
        var arguments = context.Target.Split(",");
        if (arguments.Length < 2)
        {
            return;
        }

        var command = arguments[0].ToLowerAlphaNum();
        var content = arguments[1].Trim();

        if (command.Length > 15)
        {
            context.Reply("Command name is too long.");
            return;
        }

        if (content.Length > 300)
        {
            context.Reply("Command content is too long");
            return;
        }

        if (content.StartsWith(_configurationManager.Configuration.Trigger) || content.StartsWith("/") ||
            content.StartsWith("!"))
        {
            context.Reply("The command's content cannot start with this character");
            return;
        }

        var existingCommand = await _addedCommandRepository.GetByIdAsync(command);
        if (existingCommand != null)
        {
            context.Reply("The command already exists.");
            return;
        }

        await _addedCommandRepository.AddAsync(new AddedCommand
        {
            Author = context.Sender.Name,
            Content = content,
            CreationDate = DateTime.Now,
            Id = command
        });
        await _addedCommandRepository.Save();
        
        context.Reply($"Added command '{command}'");
    }
}