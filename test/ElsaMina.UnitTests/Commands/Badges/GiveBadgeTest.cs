using ElsaMina.Commands.Badges;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.RoomUserData;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.Badges;

public class GiveBadgeTest
{
    private IContext _context;
    private IBadgeRepository _badgeRepository;
    private IRoomUserDataService _roomUserDataService;
    private GiveBadge _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _badgeRepository = Substitute.For<IBadgeRepository>();
        _roomUserDataService = Substitute.For<IRoomUserDataService>();
        _command = new GiveBadge(_badgeRepository, _roomUserDataService);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithHelpMessage_WhenInvalidArguments()
    {
        // Arrange
        _context.GetString("badge_give_help_message").Returns("myMessage");
        _context.Target.Returns("invalid");
        _context.RoomId.Returns("roomId");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received().Reply("myMessage");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithCouldNotFindBadgeMessage_WhenBadgeNotFound()
    {
        // Arrange
        _context.Target.Returns("userId,nonExistingBadge");
        _context.RoomId.Returns("roomId");
        _badgeRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns((Badge)null);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("badge_give_could_not_find_badge", "nonexistingbadge");
    }

    [Test]
    public async Task Test_RunAsync_ShouldGiveBadgeToUserAndReplyWithSuccessMessage_WhenValidArguments()
    {
        // Arrange
        var badgeId = "existingbadge";
        var badgeName = "Existing Badge";
        _context.Target.Returns("userId,existingBadge");
        _context.RoomId.Returns("roomId");
        var existingBadge = new Badge { Id = badgeId, Name = badgeName };
        _badgeRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(existingBadge);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _roomUserDataService.Received().GiveBadgeToUser("roomId", "userid", badgeId);
        _context.Received().ReplyLocalizedMessage("badge_give_success", "userid", badgeName);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithErrorMessage_WhenExceptionThrown()
    {
        // Arrange
        _context.Target.Returns("userId,existingBadge");
        _context.RoomId.Returns("roomId");
        var existingBadge = new Badge { Id = "existingbadge" };
        _badgeRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(existingBadge);
        _roomUserDataService.GiveBadgeToUser("roomId", "userid", "existingbadge")
            .Throws(new Exception("Some error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("badge_give_error", "Some error");
    }
}