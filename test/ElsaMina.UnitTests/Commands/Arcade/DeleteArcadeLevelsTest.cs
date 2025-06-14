using ElsaMina.Commands.Arcade;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Arcade;

public class DeleteArcadeLevelTests
{
    private DeleteArcadeLevel _command;
    private IArcadeLevelRepository _arcadeLevelRepository;

    [SetUp]
    public void SetUp()
    {
        _arcadeLevelRepository = Substitute.For<IArcadeLevelRepository>();
        _command = new DeleteArcadeLevel(_arcadeLevelRepository);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyHelpMessage_WhenTargetIsNullOrWhitespace()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns(string.Empty);

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).GetString("arcade_level_delete_help");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyNotFound_WhenArcadeLevelDoesNotExist()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns("nonexistentuser");
        _arcadeLevelRepository.GetByIdAsync("nonexistentuser").Returns((ArcadeLevel)null);

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("arcade_level_delete_not_found");
    }

    [Test]
    public async Task Test_RunAsync_ShouldDeleteArcadeLevel_WhenArcadeLevelExists()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns("existinguser");
        _arcadeLevelRepository.GetByIdAsync("existinguser").Returns(new ArcadeLevel { Id = "existing_user" });

        // Act
        await _command.RunAsync(context);

        // Assert
        await _arcadeLevelRepository.Received(1).DeleteByIdAsync("existinguser");
        context.Received(1).ReplyLocalizedMessage("arcade_level_delete_success");
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleExceptionAndReplyError_WhenDeleteFails()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        context.Target.Returns("usertodelete");
        _arcadeLevelRepository.GetByIdAsync("usertodelete").Returns(new ArcadeLevel { Id = "usertodelete" });
        _arcadeLevelRepository
            .When(repo => repo.DeleteByIdAsync("usertodelete"))
            .Throw(new Exception("Database error"));

        // Act
        await _command.RunAsync(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("arcade_level_delete_failure", "Database error");
    }
}
