using System.Globalization;
using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.Test.Core.Contexts;

public class PmContextTests
{
    private IContextProvider _contextProvider;
    private IBot _bot;
    private IUser _sender;
    private string _message;
    private string _target;
    private string _command;

    [SetUp]
    public void SetUp()
    {
        // Mock dependencies
        _contextProvider = Substitute.For<IContextProvider>();
        _bot = Substitute.For<IBot>();
        _sender = Substitute.For<IUser>();
        _message = "Test message";
        _target = "Test target";
        _command = "Test command";

        // Mock IContextProvider
        _contextProvider.DefaultCulture.Returns(CultureInfo.InvariantCulture);
        _contextProvider.DefaultRoom.Returns("TestRoom");
    }

    [Test]
    public void Test_PmContext_ShouldHavePrivateMessageFlag()
    {
        // Act
        var context = new PmContext(_contextProvider, _bot, _message, _target, _sender, _command);

        // Assert
        Assert.That(context.IsPrivateMessage, Is.True);
    }

    [Test]
    public void Test_PmContext_ShouldReturnCorrectRoomId()
    {
        // Act
        var context = new PmContext(_contextProvider, _bot, _message, _target, _sender, _command);

        // Assert
        Assert.That(context.RoomId, Is.EqualTo("TestRoom"));
    }

    [Test]
    public void Test_PmContext_ShouldReturnCorrectCulture()
    {
        // Act
        var context = new PmContext(_contextProvider, _bot, _message, _target, _sender, _command);

        // Assert
        Assert.That(context.Culture, Is.EqualTo(CultureInfo.InvariantCulture));
    }

    [Test]
    public void Test_PmContext_ShouldCallBotSendForReply()
    {
        // Arrange
        var context = new PmContext(_contextProvider, _bot, _message, _target, _sender, _command);
        var replyMessage = "Test reply";

        // Act
        context.Reply(replyMessage);

        // Assert
        _bot.Received().Send($"|/pm {_sender.UserId}, {replyMessage}");
    }

    [Test]
    public void Test_PmContext_ShouldCallBotSendForSendHtml()
    {
        // Arrange
        var context = new PmContext(_contextProvider, _bot, _message, _target, _sender, _command);
        var htmlContent = "<div>Test HTML</div>";

        // Act
        context.ReplyHtml(htmlContent);

        // Assert
        _bot.Received().Say("TestRoom", $"/pminfobox {_sender.UserId}, {htmlContent}");
    }

    [Test]
    public void Test_PmContext_ShouldCallBotSendForSendUpdatableHtml()
    {
        // Arrange
        var context = new PmContext(_contextProvider, _bot, _message, _target, _sender, _command);
        var htmlId = "123";
        var htmlContent = "<div>Updatable HTML</div>";

        // Act
        context.ReplyUpdatableHtml(htmlId, htmlContent, true);

        // Assert
        _bot.Received().Say("TestRoom", $"/pmchangeuhtml {_sender.UserId}, {htmlId}, {htmlContent}");
    }

    [Test]
    public void Test_PmContext_ShouldAlwaysHaveSufficientRank([Values] Rank rank)
    {
        // Act
        var context = new PmContext(_contextProvider, _bot, _message, _target, _sender, _command);

        // Assert
        Assert.That(context.HasSufficientRank(rank), Is.True);
    }
}