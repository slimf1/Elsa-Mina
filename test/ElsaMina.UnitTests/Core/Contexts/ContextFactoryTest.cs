using System.Reflection;
using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.PrivateMessages;
using ElsaMina.Core.Services.Rooms;
using NSubstitute;

namespace ElsaMina.UnitTests.Core.Contexts;

public class ContextFactoryTest
{
    private ContextFactory _contextFactory;
    private IContextProvider _contextProvider;
    private IBot _bot;
    private IRoomsManager _roomsManager;
    private IConfigurationManager _configurationManager;
    private IPmSendersManager _pmSendersManager;

    [SetUp]
    public void SetUp()
    {
        _contextProvider = Substitute.For<IContextProvider>();
        _bot = Substitute.For<IBot>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _configurationManager = Substitute.For<IConfigurationManager>();
        _pmSendersManager = Substitute.For<IPmSendersManager>();

        _contextFactory = new ContextFactory(_contextProvider, _bot, _roomsManager, _configurationManager, _pmSendersManager);
    }

    [Test]
    public void Test_TryBuildContextFromReceivedMessage_ShouldReturnNull_WhenPartsAreInvalid()
    {
        // Arrange
        string[] parts = ["invalid"];

        // Act
        var result = _contextFactory.TryBuildContextFromReceivedMessage(parts);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Test_TryBuildContextFromReceivedMessage_ShouldReturnNull_WhenRoomIsNull()
    {
        // Arrange
        string[] parts = ["", "c:", "1234567890", "user1", "!test"];
        _roomsManager.GetRoom("room1").Returns((IRoom)null);

        // Act
        var result = _contextFactory.TryBuildContextFromReceivedMessage(parts, "room1");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Test_TryBuildContextFromReceivedMessage_ShouldReturnRoomContext_WhenValidRoomMessage()
    {
        // Arrange
        string[] parts = ["", "c:", "1234567890", "user1", "!test arg"];
        var mockRoom = Substitute.For<IRoom>();
        var mockUser = Substitute.For<IUser>();

        mockRoom.Users["user1"].Returns(mockUser);
        _roomsManager.GetRoom("room1").Returns(mockRoom);
        _configurationManager.Configuration.Trigger.Returns("!");
        
        // Act
        var result = _contextFactory.TryBuildContextFromReceivedMessage(parts, "room1");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<RoomContext>());
            Assert.That(((RoomContext)result).Command, Is.EqualTo("test"));
            Assert.That(((RoomContext)result).Target, Is.EqualTo("arg"));
        });
    }

    [Test]
    public void Test_TryBuildContextFromReceivedMessage_ShouldReturnPmContext_WhenValidPmMessage()
    {
        // Arrange
        string[] parts = ["", "pm", "user1", "someData", "!test arg"];
        var mockUser = Substitute.For<IUser>();

        _pmSendersManager.GetUser("user1").Returns(mockUser);
        _configurationManager.Configuration.Trigger.Returns("!");

        // Act
        var result = _contextFactory.TryBuildContextFromReceivedMessage(parts);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<PmContext>());
            Assert.That(((PmContext)result).Command, Is.EqualTo("test"));
            Assert.That(((PmContext)result).Target, Is.EqualTo("arg"));
        });
    }

    [Test]
    public void Test_GetTargetAndCommand_ShouldReturnParsedCommand_WhenMessageStartsWithTrigger()
    {
        // Arrange
        var trigger = "!";
        _configurationManager.Configuration.Trigger.Returns(trigger);
        var message = "!command arg";

        // Act
        var result = typeof(ContextFactory)
            .GetMethod("GetTargetAndCommand", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(_contextFactory, new object[] { message });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            var (target, command) = ((string, string))result;
            Assert.That(target, Is.EqualTo("arg"));
            Assert.That(command, Is.EqualTo("command"));
        });
    }

    [Test]
    public void Test_GetTargetAndCommand_ShouldReturnNull_WhenMessageDoesNotStartWithTrigger()
    {
        // Arrange
        var trigger = "!";
        _configurationManager.Configuration.Trigger.Returns(trigger);
        var message = "command arg";

        // Act
        var result = typeof(ContextFactory)
            .GetMethod("GetTargetAndCommand", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(_contextFactory, [message]);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            var (target, command) = ((string, string))result;
            Assert.That(target, Is.Null);
            Assert.That(command, Is.Null);
        });
    }
}