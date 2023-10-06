using ElsaMina.Commands.CustomCommands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using ElsaMina.Test.Fixtures;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.Test.Commands.CustomCommands;

public class AddCustomCommandTest
{
    private IAddedCommandRepository _addedCommandRepository;
    private IConfigurationManager _configurationManager;
    private IClockService _clockService;

    private AddCustomCommand _addCustomCommand;

    [SetUp]
    public void SetUp()
    {
        _addedCommandRepository = Substitute.For<IAddedCommandRepository>();
        _configurationManager = Substitute.For<IConfigurationManager>();
        _clockService = Substitute.For<IClockService>();

        _addCustomCommand = new AddCustomCommand(_addedCommandRepository, _configurationManager, _clockService);
    }

    [Test]
    public async Task Test_Run_ShouldAddCommandToDatabase_WhenHasValidArguments()
    {
        // Arrange
        var date = new DateTime(2022, 2, 3);
        _clockService.CurrentUtcDateTime.Returns(date);
        var context = Substitute.For<IContext>();
        context.Target.Returns("test-command,Test command content");
        context.Sender.Returns(UserFixtures.VoicedUser("John"));
        context.RoomId.Returns("room-1");
        _configurationManager.Configuration.Returns(new Configuration { Trigger = "+" });
        _addedCommandRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).ReturnsNull();

        // Act
        await _addCustomCommand.Run(context);

        // Assert
        await _addedCommandRepository.Received(1)
            .AddAsync(Arg.Is<AddedCommand>(c =>
                c.Id == "test-command" &&
                c.Content == "Test command content" &&
                c.RoomId == "room-1" &&
                c.Author == "John" &&
                c.CreationDate == date));
        context.Received(1).ReplyLocalizedMessage("addcommand_success", "test-command");
    }

    [Test]
    public async Task Test_Run_ShouldNotAddCommandToDatabase_WhenCommandNameIsTooLong()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns("very-long-command-name,Test command content");

        // Act
        await _addCustomCommand.Run(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("addcommand_name_too_long");
        await _addedCommandRepository.DidNotReceive().AddAsync(Arg.Any<AddedCommand>());
    }

    [Test]
    public async Task Test_Run_ShouldNotAddCommandToDatabase_WhenCommandContentIsTooLong()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns("test-command," + new string('a', 301));

        // Act
        await _addCustomCommand.Run(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("addcommand_content_too_long");
        await _addedCommandRepository.DidNotReceive().AddAsync(Arg.Any<AddedCommand>());
    }

    [Test]
    public async Task Test_Run_ShouldNotAddCommandToDatabase_WhenCommandContentStartsWithInvalidCharacter()
    {
        // Arrange
        _configurationManager.Configuration.Trigger.Returns("@");
        var context = Substitute.For<IContext>();
        context.Target.Returns("test-command,@Test command content");

        // Act
        await _addCustomCommand.Run(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("addcommand_bad_first_char");
        await _addedCommandRepository.DidNotReceive().AddAsync(Arg.Any<AddedCommand>());
    }

    [Test]
    public async Task Test_Run_ShouldNotAddCommandToDatabase_WhenCommandAlreadyExists()
    {
        // Arrange
        _configurationManager.Configuration.Trigger.Returns("@");
        var existingCommand = new AddedCommand { Id = "existing", RoomId = "room-1" };
        _addedCommandRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(existingCommand);
        var context = Substitute.For<IContext>();
        context.Target.Returns("existing,Test command content");

        // Act
        await _addCustomCommand.Run(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("addcommand_already_exist");
        await _addedCommandRepository.DidNotReceive().AddAsync(Arg.Any<AddedCommand>());
    }
}