using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Commands;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.DataAccess;
using ElsaMina.Logging;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.Commands.CustomCommands;

[NamedCommand("randcom", "randcustom", "rand-custom")]
public class RandomCustomCommand : Command
{
    private readonly IBotDbContextFactory _dbContextFactory;
    private readonly IRandomService _randomService;

    public RandomCustomCommand(IBotDbContextFactory dbContextFactory, IRandomService randomService)
    {
        _dbContextFactory = dbContextFactory;
        _randomService = randomService;
    }

    public override async Task RunAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var commands = await dbContext
                .AddedCommands
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            var randomCommand = _randomService.RandomElement(commands);
            context.Reply(randomCommand.Content);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Error while fetching random custom command");
            context.ReplyLocalizedMessage("randcustom_error", exception.Message);
        }
    }
}