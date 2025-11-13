using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;

namespace ElsaMina.Commands.CustomCommands;

[NamedCommand("add-custom-command", Aliases = ["add-custom", "add-command"])]
public class AddCustomCommand : Command
{
    private const int MAX_COMMAND_NAME_LENGTH = 18;
    private const int MAX_CONTENT_LENGTH = 300;

    private readonly IAddedCommandRepository _addedCommandRepository;
    private readonly IConfiguration _configuration;
    private readonly IClockService _clockService;

    public AddCustomCommand(IAddedCommandRepository addedCommandRepository,
        IConfiguration configuration,
        IClockService clockService)
    {
        _addedCommandRepository = addedCommandRepository;
        _configuration = configuration;
        _clockService = clockService;
    }

    public override Rank RequiredRank => Rank.Driver;

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        var arguments = context.Target.Split(",");
        if (arguments.Length < 2)
        {
            return;
        }

        var command = arguments[0].Trim().ToLower();
        var content = string.Join(",", arguments[1..]).Trim();

        if (command.Length > MAX_COMMAND_NAME_LENGTH)
        {
            context.ReplyLocalizedMessage("addcommand_name_too_long");
            return;
        }

        if (content.Length > MAX_CONTENT_LENGTH)
        {
            context.ReplyLocalizedMessage("addcommand_content_too_long");
            return;
        }

        if (content.StartsWith(_configuration.Trigger) || content.StartsWith('/') ||
            content.StartsWith('!'))
        {
            context.ReplyLocalizedMessage("addcommand_bad_first_char");
            return;
        }

        var existingCommand = await _addedCommandRepository.GetByIdAsync(Tuple.Create(
            command, context.RoomId), cancellationToken);
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
        }, cancellationToken);
        await _addedCommandRepository.SaveChangesAsync(cancellationToken);

        context.ReplyLocalizedMessage("addcommand_success", command);
    }
}