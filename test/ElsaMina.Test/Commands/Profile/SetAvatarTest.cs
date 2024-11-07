using ElsaMina.Commands.Profile;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.RoomUserData;
using NSubstitute;

namespace ElsaMina.Test.Commands.Profile;

public class SetAvatarTest
{
    private SetAvatar _command;
    private IRoomUserDataService _roomUserDataService;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _roomUserDataService = Substitute.For<IRoomUserDataService>();
        _command = new SetAvatar(_roomUserDataService);
        _context = Substitute.For<IContext>();
    }

    [Test]
    public async Task Test_Run_ShouldSetAvatar_WhenValidParametersProvided()
    {
        // Arrange
        _context.RoomId.Returns("testRoom");
        _context.Target.Returns("user123, https://avatar.url/image.png");

        // Act
        await _command.Run(_context);

        // Assert
        await _roomUserDataService.Received(1).SetUserAvatar("testRoom", "user123", "https://avatar.url/image.png");
        _context.Received(1).ReplyLocalizedMessage("avatar_success");
    }

    [Test]
    public async Task Test_Run_ShouldReturnHelpMessage_WhenIncorrectParameterCount()
    {
        // Arrange
        _context.Target.Returns("user123");

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage(_command.HelpMessageKey);
        await _roomUserDataService.DidNotReceiveWithAnyArgs().SetUserAvatar(default, default, default);
    }

    [Test]
    public async Task Test_Run_ShouldHandleExceptionAndSendFailureMessage_WhenSetAvatarFails()
    {
        // Arrange
        _context.RoomId.Returns("testRoom");
        _context.Target.Returns("user123, https://avatar.url/image.png");
        var errorMessage = "Database error";
        _roomUserDataService
            .When(x => x.SetUserAvatar("testRoom", "user123", "https://avatar.url/image.png"))
            .Do(x => throw new Exception(errorMessage));

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("avatar_failure", errorMessage);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeCorrect()
    {
        // Assert
        Assert.That(_command.RequiredRank, Is.EqualTo('%'));
    }

    [Test]
    public void Test_HelpMessageKey_ShouldBeCorrect()
    {
        // Assert
        Assert.That(_command.HelpMessageKey, Is.EqualTo("avatar_help_message"));
    }
}