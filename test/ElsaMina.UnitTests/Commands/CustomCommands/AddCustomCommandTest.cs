using ElsaMina.Commands.CustomCommands;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Clock;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using ElsaMina.UnitTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.CustomCommands;

public class AddCustomCommandTest
{
    private IConfiguration _configuration;
    private IClockService _clockService;
    private IBotDbContextFactory _dbContextFactory;
    private BotDbContext _dbContext;
    private DbSet<AddedCommand> _addedCommandsDbSet;
    private IContext _context;
    private AddCustomCommand _command;

    [SetUp]
    public void SetUp()
    {
        _configuration = Substitute.For<IConfiguration>();
        _clockService = Substitute.For<IClockService>();
        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContext = Substitute.For<BotDbContext>();
        _addedCommandsDbSet = Substitute.For<DbSet<AddedCommand>>();
        _context = Substitute.For<IContext>();

        _dbContext.AddedCommands.Returns(_addedCommandsDbSet);
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(_dbContext);

        _command = new AddCustomCommand(_configuration, _clockService, _dbContextFactory);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeDriver()
    {
        // Assert
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public async Task Test_RunAsync_ShouldAddCommandToDatabase_WhenHasValidArguments()
    {
        // Arrange
        var date = new DateTime(2022, 2, 3);
        _clockService.CurrentUtcDateTime.Returns(date);
        _context.Target.Returns("test-command,Test command content");
        _context.Sender.Returns(UserFixtures.VoicedUser("John"));
        _context.RoomId.Returns("room-1");
        _configuration.Trigger.Returns("+");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).AddAsync(
            Arg.Is<AddedCommand>(c =>
                c.Id == "test-command" &&
                c.Content == "Test command content" &&
                c.RoomId == "room-1" &&
                c.Author == "John" &&
                c.CreationDate == date),
            Arg.Any<CancellationToken>());
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _context.Received(1).ReplyLocalizedMessage("addcommand_success", "test-command");
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotAddCommandToDatabase_WhenCommandNameIsTooLong()
    {
        // Arrange
        var longName = new string('a', 19); // MAX_COMMAND_NAME_LENGTH is 18
        _context.Target.Returns($"{longName},Test command content");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("addcommand_name_too_long");
        await _addedCommandsDbSet.DidNotReceive().AddAsync(Arg.Any<AddedCommand>(), Arg.Any<CancellationToken>());
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotAddCommandToDatabase_WhenCommandContentIsTooLong()
    {
        // Arrange
        var longContent = new string('a', 301); // MAX_CONTENT_LENGTH is 300
        _context.Target.Returns($"test-command,{longContent}");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("addcommand_content_too_long");
        await _addedCommandsDbSet.DidNotReceive().AddAsync(Arg.Any<AddedCommand>(), Arg.Any<CancellationToken>());
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotAddCommandToDatabase_WhenCommandContentStartsWithTrigger()
    {
        // Arrange
        _configuration.Trigger.Returns("@");
        _context.Target.Returns("test-command,@Test command content");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("addcommand_bad_first_char");
        await _addedCommandsDbSet.DidNotReceive().AddAsync(Arg.Any<AddedCommand>(), Arg.Any<CancellationToken>());
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotAddCommandToDatabase_WhenCommandContentStartsWithSlash()
    {
        // Arrange
        _configuration.Trigger.Returns("+");
        _context.Target.Returns("test-command,/Test command content");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("addcommand_bad_first_char");
        await _addedCommandsDbSet.DidNotReceive().AddAsync(Arg.Any<AddedCommand>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotAddCommandToDatabase_WhenCommandContentStartsWithExclamation()
    {
        // Arrange
        _configuration.Trigger.Returns("+");
        _context.Target.Returns("test-command,!Test command content");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("addcommand_bad_first_char");
        await _addedCommandsDbSet.DidNotReceive().AddAsync(Arg.Any<AddedCommand>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotAddCommandToDatabase_WhenCommandAlreadyExists()
    {
        // Arrange
        _configuration.Trigger.Returns("@");
        var existingCommand = new AddedCommand { Id = "existing", RoomId = "room-1" };
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _context.Target.Returns("existing,Test command content");
        _context.RoomId.Returns("room-1");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("addcommand_already_exist");
        await _addedCommandsDbSet.DidNotReceive().AddAsync(Arg.Any<AddedCommand>(), Arg.Any<CancellationToken>());
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotAddCommandToDatabase_WhenTargetHasNoComma()
    {
        // Arrange
        _context.Target.Returns("test-command-without-comma");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
        _context.DidNotReceive().ReplyLocalizedMessage(Arg.Any<string>(), Arg.Any<object[]>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotAddCommandToDatabase_WhenTargetIsEmpty()
    {
        // Arrange
        _context.Target.Returns(string.Empty);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldTrimAndLowercaseCommandName()
    {
        // Arrange
        var date = new DateTime(2022, 2, 3);
        _clockService.CurrentUtcDateTime.Returns(date);
        _context.Target.Returns("  TEST-Command  ,content");
        _context.Sender.Returns(UserFixtures.VoicedUser("John"));
        _context.RoomId.Returns("room-1");
        _configuration.Trigger.Returns("+");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).AddAsync(
            Arg.Is<AddedCommand>(c => c.Id == "test-command"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldTrimContent()
    {
        // Arrange
        var date = new DateTime(2022, 2, 3);
        _clockService.CurrentUtcDateTime.Returns(date);
        _context.Target.Returns("test,  content with spaces  ");
        _context.Sender.Returns(UserFixtures.VoicedUser("John"));
        _context.RoomId.Returns("room-1");
        _configuration.Trigger.Returns("+");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).AddAsync(
            Arg.Is<AddedCommand>(c => c.Content == "content with spaces"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleContentWithMultipleCommas()
    {
        // Arrange
        var date = new DateTime(2022, 2, 3);
        _clockService.CurrentUtcDateTime.Returns(date);
        _context.Target.Returns("test,content,with,commas");
        _context.Sender.Returns(UserFixtures.VoicedUser("John"));
        _context.RoomId.Returns("room-1");
        _configuration.Trigger.Returns("+");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).AddAsync(
            Arg.Is<AddedCommand>(c => c.Content == "content,with,commas"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldFindExistingCommand_UsingCommandIdAndRoomId()
    {
        // Arrange
        _configuration.Trigger.Returns("+");
        _context.Target.Returns("test,content");
        _context.RoomId.Returns("specific-room");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).FindAsync(
            Arg.Is<object[]>(arr =>
                arr.Length == 2 &&
                arr[0].ToString() == "test" &&
                arr[1].ToString() == "specific-room"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldPassCancellationToken_ToAllDbOperations()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var date = new DateTime(2022, 2, 3);
        _clockService.CurrentUtcDateTime.Returns(date);
        _context.Target.Returns("test,content");
        _context.Sender.Returns(UserFixtures.VoicedUser("John"));
        _context.RoomId.Returns("room-1");
        _configuration.Trigger.Returns("+");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context, cancellationToken);

        // Assert
        await _dbContextFactory.Received(1).CreateDbContextAsync(cancellationToken);
        await _addedCommandsDbSet.Received(1).FindAsync(Arg.Any<object[]>(), cancellationToken);
        await _addedCommandsDbSet.Received(1).AddAsync(Arg.Any<AddedCommand>(), cancellationToken);
        await _dbContext.Received(1).SaveChangesAsync(cancellationToken);
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_AfterSuccessfulExecution()
    {
        // Arrange
        var date = new DateTime(2022, 2, 3);
        _clockService.CurrentUtcDateTime.Returns(date);
        _context.Target.Returns("test,content");
        _context.Sender.Returns(UserFixtures.VoicedUser("John"));
        _context.RoomId.Returns("room-1");
        _configuration.Trigger.Returns("+");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_WhenCommandAlreadyExists()
    {
        // Arrange
        _configuration.Trigger.Returns("+");
        var existingCommand = new AddedCommand { Id = "existing", RoomId = "room-1" };
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(existingCommand);
        _context.Target.Returns("existing,content");
        _context.RoomId.Returns("room-1");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldStoreSenderName_InAuthorField()
    {
        // Arrange
        var date = new DateTime(2022, 2, 3);
        _clockService.CurrentUtcDateTime.Returns(date);
        _context.Target.Returns("test,content");
        _context.Sender.Returns(UserFixtures.VoicedUser("Alice"));
        _context.RoomId.Returns("room-1");
        _configuration.Trigger.Returns("+");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).AddAsync(
            Arg.Is<AddedCommand>(c => c.Author == "Alice"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldStoreCurrentUtcDateTime_InCreationDateField()
    {
        // Arrange
        var specificDate = new DateTime(2023, 5, 15, 10, 30, 45);
        _clockService.CurrentUtcDateTime.Returns(specificDate);
        _context.Target.Returns("test,content");
        _context.Sender.Returns(UserFixtures.VoicedUser("John"));
        _context.RoomId.Returns("room-1");
        _configuration.Trigger.Returns("+");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).AddAsync(
            Arg.Is<AddedCommand>(c => c.CreationDate == specificDate),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldAcceptCommandNameAtMaxLength()
    {
        // Arrange
        var maxLengthName = new string('a', 18); // MAX_COMMAND_NAME_LENGTH is 18
        var date = new DateTime(2022, 2, 3);
        _clockService.CurrentUtcDateTime.Returns(date);
        _context.Target.Returns($"{maxLengthName},content");
        _context.Sender.Returns(UserFixtures.VoicedUser("John"));
        _context.RoomId.Returns("room-1");
        _configuration.Trigger.Returns("+");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).AddAsync(Arg.Any<AddedCommand>(), Arg.Any<CancellationToken>());
        _context.DidNotReceive().ReplyLocalizedMessage("addcommand_name_too_long");
    }

    [Test]
    public async Task Test_RunAsync_ShouldAcceptContentAtMaxLength()
    {
        // Arrange
        var maxLengthContent = new string('a', 300); // MAX_CONTENT_LENGTH is 300
        var date = new DateTime(2022, 2, 3);
        _clockService.CurrentUtcDateTime.Returns(date);
        _context.Target.Returns($"test,{maxLengthContent}");
        _context.Sender.Returns(UserFixtures.VoicedUser("John"));
        _context.RoomId.Returns("room-1");
        _configuration.Trigger.Returns("+");
        _addedCommandsDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((AddedCommand)null);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _addedCommandsDbSet.Received(1).AddAsync(Arg.Any<AddedCommand>(), Arg.Any<CancellationToken>());
        _context.DidNotReceive().ReplyLocalizedMessage("addcommand_content_too_long");
    }
}