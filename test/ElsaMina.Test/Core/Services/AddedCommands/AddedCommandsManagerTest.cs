using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.AddedCommands;
using ElsaMina.Core.Services.Images;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.Test.Core.Services.AddedCommands;

public class AddedCommandsManagerTest
{
    private IAddedCommandRepository _addedCommandRepository;
    private AddedCommandsManager _addedCommandsManager;
    private IImageService _imageService;

    [SetUp]
    public void SetUp()
    {
        _addedCommandRepository = Substitute.For<IAddedCommandRepository>();
        _imageService = Substitute.For<IImageService>();
        _addedCommandsManager = new AddedCommandsManager(_addedCommandRepository, _imageService);
    }

    [Test]
    public async Task Test_TryExecuteAddedCommand_ShouldDoNothing_WhenCommandNotFound()
    {
        // Arrange
        var commandName = "nonExistentCommand";
        var context = Substitute.For<IContext>();
        _addedCommandRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).ReturnsNull();

        // Act
        await _addedCommandsManager.TryExecuteAddedCommand(commandName, context);

        // Assert
        context.DidNotReceive().SendHtml(Arg.Any<string>(),  rankAware: Arg.Any<bool>());
        context.DidNotReceive().Reply(Arg.Any<string>(), rankAware: Arg.Any<bool>());
    }

    [Test]
    public async Task Test_TryExecuteAddedCommand_ShouldSendImage_WhenContentIsImageUrl()
    {
        // Arrange
        var commandName = "imageCommand";
        var context = Substitute.For<IContext>();
        var command = new AddedCommand
        {
            Content = "https://example.com/image.png"
        };
        _addedCommandRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(command);
        _imageService.GetRemoteImageDimensions(Arg.Any<string>()).Returns(Task.FromResult((400, 300)));
        _imageService.ResizeWithSameAspectRatio(400, 300, Arg.Any<int>(), Arg.Any<int>()).Returns((400, 300));

        // Act
        await _addedCommandsManager.TryExecuteAddedCommand(commandName, context);

        // Assert
        context.Received().SendHtml(Arg.Is<string>(s => s.Contains("https://example.com/image.png")), rankAware: Arg.Any<bool>());
    }

    [Test]
    public async Task Test_TryExecuteAddedCommand_ShouldReplyWithContent_WhenContentIsText()
    {
        // Arrange
        var commandName = "textCommand";
        var context = Substitute.For<IContext>();
        var command = new AddedCommand
        {
            Content = "This is a command response!"
        };
        _addedCommandRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(command);

        // Act
        await _addedCommandsManager.TryExecuteAddedCommand(commandName, context);

        // Assert
        context.Received().Reply("This is a command response!", Arg.Any<bool>());
    }

    [Test]
    public async Task Test_TryExecuteAddedCommand_ShouldResizeImage_WhenImageDimensionsExceedMax()
    {
        // Arrange
        var commandName = "largeImageCommand";
        var context = Substitute.For<IContext>();
        var command = new AddedCommand
        {
            Content = "https://example.com/largeimage.png"
        };
        _addedCommandRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(command);
        _imageService.GetRemoteImageDimensions(Arg.Any<string>()).Returns(Task.FromResult((800, 600)));
        _imageService.ResizeWithSameAspectRatio(800, 600, 400, 300).Returns((400, 300));

        // Act
        await _addedCommandsManager.TryExecuteAddedCommand(commandName, context);

        // Assert
        context.Received().SendHtml(Arg.Is<string>(s => s.Contains("width=\"400\" height=\"300\"")), rankAware: Arg.Any<bool>());
    }

    [Test]
    public async Task Test_TryExecuteAddedCommand_ShouldNotResizeImage_WhenImageDimensionsAreWithinMax()
    {
        // Arrange
        var commandName = "smallImageCommand";
        var context = Substitute.For<IContext>();
        var command = new AddedCommand
        {
            Content = "https://example.com/smallimage.png"
        };
        _addedCommandRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(command);
        _imageService.GetRemoteImageDimensions(Arg.Any<string>()).Returns(Task.FromResult((200, 150)));
        _imageService.ResizeWithSameAspectRatio(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>()).Returns((200, 150));

        // Act
        await _addedCommandsManager.TryExecuteAddedCommand(commandName, context);

        // Assert
        context.Received().SendHtml(Arg.Is<string>(s => s.Contains("width=\"200\" height=\"150\"")), rankAware: Arg.Any<bool>());
    }
}