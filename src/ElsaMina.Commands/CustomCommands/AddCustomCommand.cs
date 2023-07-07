using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.CustomCommands;

public class AddCustomCommand : BaseCommand<AddCustomCommand>, INamed
{
    public static string Name => "add-custom-command";
    public static IEnumerable<string> Aliases => new[] { "add-custom", "add-command" };

    private readonly IRepository<AddedCommand, Tuple<string, string>> _addedCommandRepository;
    private readonly IConfigurationManager _configurationManager;
    private readonly IClockService _clockService;
    
    public AddCustomCommand(IRepository<AddedCommand, Tuple<string, string>> addedCommandRepository,
        IConfigurationManager configurationManager,
        IClockService clockService)
    {
        _addedCommandRepository = addedCommandRepository;
        _configurationManager = configurationManager;
        _clockService = clockService;
    }
    
    public override char RequiredRank => '@';

    public override async Task Run(IContext context)
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
            context.ReplyLocalizedMessage("addcommand_name_too_long");
            return;
        }

        if (content.Length > 300)
        {
            context.ReplyLocalizedMessage("addcommand_content_too_long");
            return;
        }

        if (content.StartsWith(_configurationManager.Configuration.Trigger) || content.StartsWith("/") ||
            content.StartsWith("!"))
        {
            context.ReplyLocalizedMessage("addcommand_bad_first_char");
            return;
        }

        var existingCommand = await _addedCommandRepository.GetByIdAsync(new Tuple<string, string>(
            command, context.RoomId));
        if (existingCommand != null)
        {
            context.ReplyLocalizedMessage("addcommand_already_exist");
            return;
        }

        await _addedCommandRepository.AddAsync(new AddedCommand
        {
            Author = context.Sender.Name,
            Content = content,
            RoomId = context.RoomId,
            CreationDate = _clockService.CurrentUtcDateTime,
            Id = command
        });
        
        context.ReplyLocalizedMessage("addcommand_success", command);
    }
}