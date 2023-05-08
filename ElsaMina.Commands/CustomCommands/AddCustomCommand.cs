using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
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
    private readonly IClockService _clockService;
    
    public AddCustomCommand(IAddedCommandRepository addedCommandRepository,
        IConfigurationManager configurationManager,
        IClockService clockService)
    {
        _addedCommandRepository = addedCommandRepository;
        _configurationManager = configurationManager;
        _clockService = clockService;
    }

    public async Task Run(IContext context)
    {
        var arguments = context.Target.Split(",");
        if (arguments.Length < 2)
        {
            return;
        }

        var command = arguments[0].Trim().ToLower();
        var content = arguments[1].Trim();

        if (command.Length > 18)
        {
            context.Reply(context.GetString("addcommand_name_too_long"));
            return;
        }

        if (content.Length > 300)
        {
            context.Reply(context.GetString("addcommand_content_too_long"));
            return;
        }

        if (content.StartsWith(_configurationManager.Configuration.Trigger) || content.StartsWith("/") ||
            content.StartsWith("!"))
        {
            context.Reply(context.GetString("addcommand_bad_first_char"));
            return;
        }

        var existingCommand = await _addedCommandRepository.GetByIdAsync(command, context.RoomId);
        if (existingCommand != null)
        {
            context.Reply(context.GetString("addcommand_already_exist"));
            return;
        }

        await _addedCommandRepository.AddAsync(new AddedCommand
        {
            Author = context.Sender.Name,
            Content = content,
            RoomId = context.RoomId,
            CreationDate = _clockService.CurrentDateTime,
            Id = command
        });
        
        context.Reply(context.GetString("addcommand_success", command));
    }
}