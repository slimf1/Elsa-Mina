using ElsaMina.Commands.CustomCommands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.Test.Commands.CustomCommands;

public class EditCustomCommandTest
{
    private IAddedCommandRepository _addedCommandRepository;
    private IContext _context;
    private EditCustomCommand _editCustomCommand;

    [SetUp]
    public void SetUp()
    {
        _addedCommandRepository = Substitute.For<IAddedCommandRepository>();
        _context = Substitute.For<IContext>();
        _editCustomCommand = new EditCustomCommand(_addedCommandRepository);
    }

    [Test]
    public void Test_RequiredRank_ShouldBePercent()
    {
        // Assert
        Assert.That(_editCustomCommand.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithHelpMessage_WhenCommandIsNotFound()
    {
        // Arrange
        _context.Target.Returns("nonexistentcommand, new content");
        _context.RoomId.Returns("room1");
        _addedCommandRepository
            .GetByIdAsync(Arg.Any<Tuple<string, string>>())
            .ReturnsNull();

        // Act
        await _editCustomCommand.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage(_editCustomCommand.HelpMessageKey);
    }

    [Test]
    public async Task Test_RunAsync_ShouldUpdateCommandAndReplySuccess_WhenCommandExists()
    {
        // Arrange
        _context.Target.Returns("existingcommand, updated content");
        _context.RoomId.Returns("room1");
        var existingCommand = new AddedCommand { Content = "old content" };
        _addedCommandRepository
            .GetByIdAsync(Arg.Any<Tuple<string, string>>())
            .Returns(existingCommand);

        // Act
        await _editCustomCommand.RunAsync(_context);

        // Assert
        Assert.That(existingCommand.Content, Is.EqualTo("updated content"));
        await _addedCommandRepository.Received(1).UpdateAsync(existingCommand);
        _context.Received(1).ReplyLocalizedMessage("editcommand_success", "existingcommand");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithFailureMessage_WhenUpdateThrowsException()
    {
        // Arrange
        _context.Target.Returns("existingcommand, new content");
        _context.RoomId.Returns("room1");

        var existingCommand = new AddedCommand { Content = "old content" };
        _addedCommandRepository
            .GetByIdAsync(Arg.Any<Tuple<string, string>>())
            .Returns(existingCommand);

        var exceptionMessage = "Database error";
        _addedCommandRepository
            .When(repo => repo.UpdateAsync(existingCommand))
            .Do(x => throw new Exception(exceptionMessage));

        // Act
        await _editCustomCommand.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("editcommand_failure", exceptionMessage);
    }

    [Test]
    public async Task Test_RunAsync_ShouldLogErrorAndReplyWithHelpMessage_WhenGetByIdThrowsException()
    {
        // Arrange
        _context.Target.Returns("commandid, new content");
        _context.RoomId.Returns("room1");
        _addedCommandRepository
            .When(repo => repo.GetByIdAsync(Arg.Any<Tuple<string, string>>()))
            .Do(x => throw new Exception("Some error"));

        // Act
        await _editCustomCommand.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage(_editCustomCommand.HelpMessageKey);
    }
}
