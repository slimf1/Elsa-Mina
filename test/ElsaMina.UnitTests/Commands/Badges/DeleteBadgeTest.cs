using ElsaMina.Commands.Badges;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.Badges;

public class DeleteBadgeTest
{
    private IContext _context;
    private IBadgeRepository _badgeRepository;
    private DeleteBadge _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _badgeRepository = Substitute.For<IBadgeRepository>();
        _command = new DeleteBadge(_badgeRepository);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithDoesntExistMessage_WhenNonExistingBadge()
    {
        // Arrange
        _context.Target.Returns("nonExistingBadge");
        _context.RoomId.Returns("roomId");
        _badgeRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns((Badge)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("badge_delete_doesnt_exist", "nonexistingbadge");
    }

    [Test]
    public async Task Test_RunAsync_ShouldDeleteBadgeAndReplyWithSuccessMessage_WhenExistingBadge()
    {
        // Arrange
        var existingBadge = new Badge { Id = "existingbadge" };
        _context.Target.Returns("existingBadge");
        _context.RoomId.Returns("roomId");
        _badgeRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(existingBadge);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _badgeRepository.Received(1).DeleteAsync(existingBadge);
        await _badgeRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _context.Received().ReplyLocalizedMessage("badge_delete_success", "existingbadge");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithFailureMessage_WhenExceptionThrown()
    {
        // Arrange
        var existingBadge = new Badge { Id = "existingbadge" };
        _context.Target.Returns("existingBadge");
        _context.RoomId.Returns("roomId");
        _badgeRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(existingBadge);
        _badgeRepository.DeleteAsync(existingBadge).Throws(new Exception("Some error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("badge_delete_failure", "Some error");
    }
}