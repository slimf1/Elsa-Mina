using ElsaMina.Commands.JoinPhrases;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.RoomUserData;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.Test.Commands.JoinPhrases;

public class SetJoinPhraseTest
{
    private IRoomUserDataService _roomUserDataService;
    private SetJoinPhrase _command;

    private const string TEST_ROOM_ID = "testroom";
    private const string TEST_USER_ID = "testuser";
    private const string JOIN_PHRASE = "Welcome back!";

    [SetUp]
    public void SetUp()
    {
        _roomUserDataService = Substitute.For<IRoomUserDataService>();
        _command = new SetJoinPhrase(_roomUserDataService);
    }

    [Test]
    public void Test_RequiredRank_ShouldReturnCorrectRank()
    {
        // Assert
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public void Test_HelpMessageKey_ShouldReturnCorrectKey()
    {
        // Assert
        Assert.That(_command.HelpMessageKey, Is.EqualTo("setjoinphrase_help_message"));
    }

    [Test]
    public async Task Test_Run_ShouldReplyHelpMessage_WhenArgumentsAreInvalid()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns("invalidInput");

        // Act
        await _command.Run(context);

        // Assert
        context.Received(1).GetString(_command.HelpMessageKey, []);
    }

    [Test]
    public async Task Test_Run_ShouldSetJoinPhrase_WhenArgumentsAreValid()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns($"{TEST_USER_ID},{JOIN_PHRASE}");
        context.RoomId.Returns(TEST_ROOM_ID);

        // Act
        await _command.Run(context);

        // Assert
        await _roomUserDataService.Received(1).SetUserJoinPhrase(TEST_ROOM_ID, TEST_USER_ID, JOIN_PHRASE);
        context.Received(1).ReplyLocalizedMessage("setjoinphrase_success");
    }

    [Test]
    public async Task Test_Run_ShouldReplyFailureMessage_WhenExceptionOccurs()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns($"{TEST_USER_ID},{JOIN_PHRASE}");
        context.RoomId.Returns(TEST_ROOM_ID);
        
        var exception = new Exception("Database error");
        _roomUserDataService.SetUserJoinPhrase(TEST_ROOM_ID, TEST_USER_ID, JOIN_PHRASE).Throws(exception);

        // Act
        await _command.Run(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("setjoinphrase_failure", exception.Message);
    }
}