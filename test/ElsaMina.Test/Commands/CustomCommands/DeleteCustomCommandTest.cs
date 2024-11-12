using ElsaMina.Commands.CustomCommands;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;

namespace ElsaMina.Test.Commands.CustomCommands;

public class DeleteCustomCommandTest
{
    private IAddedCommandRepository _addedCommandRepository;
    private IContext _context;
    private DeleteCustomCommand _deleteCustomCommand;

    [SetUp]
    public void SetUp()
    {
        _addedCommandRepository = Substitute.For<IAddedCommandRepository>();
        _context = Substitute.For<IContext>();
        _deleteCustomCommand = new DeleteCustomCommand(_addedCommandRepository);
    }

    [Test]
    public void Test_RequiredRank_ShouldBePercent()
    {
        // Assert
        Assert.That(_deleteCustomCommand.RequiredRank, Is.EqualTo('%'));
    }

    [Test]
    public async Task Test_Run_ShouldDeleteCommandAndReplySuccess_WhenCommandExists()
    {
        // Arrange
        _context.Target.Returns("commandToDelete");
        _context.RoomId.Returns("room1");

        // Act
        await _deleteCustomCommand.Run(_context);

        // Assert
        await _addedCommandRepository.Received(1)
            .DeleteAsync(new Tuple<string, string>("commandtodelete", "room1"));
        _context.Received(1).ReplyLocalizedMessage("deletecommand_success", "commandtodelete");
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithFailureMessage_WhenDeleteThrowsException()
    {
        // Arrange
        _context.Target.Returns("commandToDelete");
        _context.RoomId.Returns("room1");

        var exceptionMessage = "Database error";
        _addedCommandRepository
            .When(repo => repo.DeleteAsync(Arg.Any<Tuple<string, string>>()))
            .Do(x => throw new Exception(exceptionMessage));

        // Act
        await _deleteCustomCommand.Run(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("deletecommand_failure", exceptionMessage);
    }
}