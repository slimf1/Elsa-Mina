using System.Globalization;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.AddedCommands;
using ElsaMina.Core.Services.Images;
using ElsaMina.Core.Services.Probabilities;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Room = ElsaMina.Core.Services.Rooms.Room;
using User = ElsaMina.Core.Services.Rooms.User;

namespace ElsaMina.UnitTests.Core.Services.AddedCommands;

public class AddedCommandsManagerTest
{
    private IBotDbContextFactory _factory;
    private BotDbContext _db;
    private DbSet<AddedCommand> _set;
    private AddedCommandsManager _manager;
    private IImageService _imageService;
    private IRandomService _randomService;

    [SetUp]
    public void SetUp()
    {
        _factory = Substitute.For<IBotDbContextFactory>();
        _db = Substitute.For<BotDbContext>();
        _set = Substitute.For<DbSet<AddedCommand>>();

        _db.AddedCommands.Returns(_set);

        _factory.CreateDbContextAsync()
            .Returns(Task.FromResult(_db));

        _imageService = Substitute.For<IImageService>();
        _randomService = Substitute.For<IRandomService>();

        _manager = new AddedCommandsManager(_factory, _imageService, _randomService);
    }

    private void MockFindAsyncReturns(AddedCommand? result)
    {
        _set.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var key = callInfo.Arg<object[]>();
                return new ValueTask<AddedCommand?>(result);
            });
    }

    [Test]
    public async Task Test_TryExecuteAddedCommand_ShouldDoNothing_WhenCommandNotFound()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        MockFindAsyncReturns(null);

        // Act
        await _manager.TryExecuteAddedCommand("nonExistent", context);

        // Assert
        context.DidNotReceive().Reply(Arg.Any<string>(), rankAware: Arg.Any<bool>());
        context.DidNotReceive().ReplyHtml(Arg.Any<string>(), rankAware: Arg.Any<bool>());
    }

    [Test]
    public async Task Test_TryExecuteAddedCommand_ShouldSendImage_WhenContentIsImageUrl()
    {
        var context = Substitute.For<IContext>();
        var cmd = new AddedCommand { Content = "https://example.com/image.png" };

        MockFindAsyncReturns(cmd);

        _imageService.GetRemoteImageDimensions(Arg.Any<string>())
            .Returns(Task.FromResult((400, 300)));
        _imageService.ResizeWithSameAspectRatio(400, 300, Arg.Any<int>(), Arg.Any<int>())
            .Returns((400, 300));

        await _manager.TryExecuteAddedCommand("imageCommand", context);

        context.Received().ReplyHtml(
            Arg.Is<string>(s => s.Contains("https://example.com/image.png")),
            rankAware: Arg.Any<bool>());
    }

    [Test]
    public async Task Test_TryExecuteAddedCommand_ShouldReplyWithContent_WhenContentIsText()
    {
        var context = Substitute.For<IContext>();
        var cmd = new AddedCommand { Content = "Hello!" };

        MockFindAsyncReturns(cmd);

        await _manager.TryExecuteAddedCommand("text", context);

        context.Received().Reply("Hello!", Arg.Any<bool>());
    }

    [Test]
    public async Task Test_TryExecuteAddedCommand_ShouldResizeImage_WhenLarge()
    {
        var context = Substitute.For<IContext>();
        var cmd = new AddedCommand { Content = "https://example.com/large.png" };

        MockFindAsyncReturns(cmd);

        _imageService.GetRemoteImageDimensions(Arg.Any<string>())
            .Returns(Task.FromResult((800, 600)));
        _imageService.ResizeWithSameAspectRatio(800, 600, 400, 300)
            .Returns((400, 300));

        await _manager.TryExecuteAddedCommand("large", context);

        context.Received().ReplyHtml(
            Arg.Is<string>(s => s.Contains("width=\"400\" height=\"300\"")),
            rankAware: Arg.Any<bool>());
    }

    [Test]
    public async Task Test_TryExecuteAddedCommand_ShouldNotResizeImage_WhenSmall()
    {
        var context = Substitute.For<IContext>();
        var cmd = new AddedCommand { Content = "https://example.com/small.png" };

        MockFindAsyncReturns(cmd);

        _imageService.GetRemoteImageDimensions(Arg.Any<string>())
            .Returns(Task.FromResult((200, 150)));
        _imageService.ResizeWithSameAspectRatio(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns((200, 150));

        await _manager.TryExecuteAddedCommand("small", context);

        context.Received().ReplyHtml(
            Arg.Is<string>(s => s.Contains("width=\"200\" height=\"150\"")),
             rankAware: Arg.Any<bool>());
    }

    [Test]
    public void Test_ParseContent_ShouldParseExpression()
    {
        var context = Substitute.For<IContext>();
        const string content = "value = {sin(123) + cos(456) + tan(789)} {repeat(sin(e), 3)}";

        var result = _manager.EvaluateContent(content, context);

        Assert.That(result, Is.EqualTo(
            "value = -0.8561420972077014 0.410781290502908850.410781290502908850.41078129050290885"));
    }

    [Test]
    public void Test_ParseContent_ShouldParseExpressionWithContext()
    {
        var context = Substitute.For<IContext>();
        context.Sender.Returns(new User("Mec", Rank.Voiced));
        context.Room.Returns(new Room("Chat Room", "chatroom", CultureInfo.InvariantCulture));
        context.Command.Returns("myCmd");
        context.Target.Returns("myArgs");

        var result = _manager.EvaluateContent("{command} {author} {room} {args}", context);

        Assert.That(result, Is.EqualTo("myCmd Mec Chat Room myArgs"));
    }
}
