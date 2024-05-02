using ElsaMina.Core;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;
using NSubstitute;

namespace ElsaMina.Test.Core.Contexts;

public class RoomContextTest
{
    private IContextProvider _contextProvider;
    private IBot _bot;
    private IUser _sender;
    private IRoom _room;

    private RoomContext _roomContext;
    
    private void CreateRoomContext(string message, string target, string command, long timestamp)
    {
        _contextProvider = Substitute.For<IContextProvider>();
        _bot = Substitute.For<IBot>();
        _sender = Substitute.For<IUser>();
        _room = Substitute.For<IRoom>();
        
        _roomContext = new RoomContext(
            _contextProvider,
            _bot,
            message,
            target,
            _sender,
            command,
            _room,
            timestamp);
    }

    [Test]
    [TestCase(' ', ExpectedResult = false)]
    [TestCase('+', ExpectedResult = false)]
    [TestCase('%', ExpectedResult = false)]
    [TestCase('@', ExpectedResult = true)]
    [TestCase('*', ExpectedResult = true)]
    [TestCase('#', ExpectedResult = true)]
    [TestCase('&', ExpectedResult = true)]
    [TestCase('X', ExpectedResult = false)]
    public bool Test_HasSufficientRank_ShouldReturnTrue_WhenSenderRankIsSufficient(char userRank)
    {
        // Arrange
        CreateRoomContext("", "", "test-command", 1);
        _sender.Rank.Returns(userRank);

        // Act & Assert
        return _roomContext.HasSufficientRank('@');
    }

    [Test]
    public void Test_HasSufficientRank_ShouldReturnTrueInEveryCase_WhenSenderIsWhitelisted(
        [Values(' ', '+', '%', '@', '*', '#', '&')] char requiredRank,
        [Values(' ', '+', '%', '@', '*', '#', '&')] char userRank)
    {
        // Arrange
        CreateRoomContext("", "", "test-command", 1);
        _sender.UserId.Returns("wl-dude");
        _sender.Rank.Returns(userRank);
        _contextProvider.CurrentWhitelist.Returns(["wl-dude"]);
        
        // Act
        var value = _roomContext.HasSufficientRank(requiredRank);
        
        // Assert
        Assert.That(value, Is.True);
    }
}