using ElsaMina.Commands.Arcade;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.Arcade;

public class SetArcadeLevelTests
{
    private SetArcadeLevel _command;
    private IArcadeLevelRepository _arcadeLevelRepository;

    [SetUp]
    public void SetUp()
    {
        _arcadeLevelRepository = Substitute.For<IArcadeLevelRepository>();
        _command = new SetArcadeLevel(_arcadeLevelRepository);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyHelpMessage_WhenInputIsInvalid()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns("invalid_input");
        context.GetString("arcade_level_help").Returns("help_msg");

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).Reply("help_msg");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyError_WhenLevelIsOutOfRange()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns("user,5");

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("arcade_level_invalid_value");
    }

    [Test]
    public async Task Test_RunAsync_ShouldAddLevel_WhenUserDoesNotExist()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns("user,3");
        _arcadeLevelRepository.GetByIdAsync("user").Returns((ArcadeLevel)null);

        // Act
        await _command.RunAsync(context);

        // Assert
        await _arcadeLevelRepository.Received(1).AddAsync(Arg.Is<ArcadeLevel>(level =>
            level.Id == "user" && level.Level == 3));
        context.Received(1).ReplyLocalizedMessage("arcade_level_add", "user", 3);
    }

    [Test]
    public async Task Test_RunAsync_ShouldUpdateLevel_WhenUserExists()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns("user,2");
        var existingLevel = new ArcadeLevel { Id = "user", Level = 3 };
        _arcadeLevelRepository.GetByIdAsync("user").Returns(existingLevel);

        // Act
        await _command.RunAsync(context);

        // Assert
        Assert.That(existingLevel.Level, Is.EqualTo(2));
        await _arcadeLevelRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        context.Received(1).ReplyLocalizedMessage("arcade_level_update", "user", 2);
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleExceptionAndReplyError_WhenRepositoryAddFails()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns("user,3");
        _arcadeLevelRepository
            .When(x => x.AddAsync(Arg.Any<ArcadeLevel>()))
            .Throw(new Exception("Database error"));

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("arcade_level_update_error", "Database error");
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleExceptionAndReplyError_WhenRepositoryUpdateFails()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns("user,3");
        _arcadeLevelRepository.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Throws(new Exception("Database error"));
        _arcadeLevelRepository.GetByIdAsync("user").Returns(new ArcadeLevel { Id = "user", Level = 3 });

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("arcade_level_update_error", "Database error");
    }
}