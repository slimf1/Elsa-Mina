using ElsaMina.Commands.Profile;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.RoomUserData;
using NSubstitute;

namespace ElsaMina.Test.Commands.Profile;

public class SetTitleTests
{
    private SetTitle _command;
    private IRoomUserDataService _roomUserDataService;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _roomUserDataService = Substitute.For<IRoomUserDataService>();
        _command = new SetTitle(_roomUserDataService);
        _context = Substitute.For<IContext>();
    }

    [Test]
    public async Task Test_RunAsync_ShouldSetUserTitle_WhenValidParametersProvided()
    {
        // Arrange
        _context.RoomId.Returns("testRoom");
        _context.Target.Returns("user123, Elite Trainer");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _roomUserDataService.Received(1).SetUserTitle("testRoom", "user123", "Elite Trainer");
        _context.Received(1).ReplyLocalizedMessage("title_success");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReturnHelpMessage_WhenIncorrectParameterCount()
    {
        // Arrange
        _context.RoomId.Returns("testRoom");
        _context.Target.Returns("user123");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage(_command.HelpMessageKey);
        await _roomUserDataService.DidNotReceiveWithAnyArgs().SetUserTitle(default, default, default);
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleExceptionAndSendFailureMessage_WhenSetUserTitleFails()
    {
        // Arrange
        _context.RoomId.Returns("testRoom");
        _context.Target.Returns("user123, Elite Trainer");
        var errorMessage = "Database error";
        _roomUserDataService
            .When(x => x.SetUserTitle("testRoom", "user123", "Elite Trainer"))
            .Do(x => throw new Exception(errorMessage));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("title_failure", errorMessage);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeCorrect()
    {
        // Assert
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Voiced));
    }

    [Test]
    public void Test_HelpMessageKey_ShouldBeCorrect()
    {
        // Assert
        Assert.That(_command.HelpMessageKey, Is.EqualTo("title_help_message"));
    }
}