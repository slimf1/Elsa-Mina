using ElsaMina.Commands.Badges;
using ElsaMina.Core.Contexts;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.Test.Commands.Badges;

public class AddBadgeTest
{
    private IContext _context;
    private IBadgeRepository _badgeRepository;
    private AddBadge _command;
    
    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _badgeRepository = Substitute.For<IBadgeRepository>();
        _command = new AddBadge(_badgeRepository);
    }
    
    [Test]
    public async Task Test_Run_ShouldReplyWithHelpMessage_WhenArgumentsAreInvalid()
    {
        // Arrange
        _context.Target.Returns("invalid");

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("badge_help_message");
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithAlreadyExistMessage_WhenBadgeExists()
    {
        // Arrange
        _context.Target.Returns("existing, image");
        _context.RoomId.Returns("roomId");
        _badgeRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).Returns(new Badge());

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("badge_add_already_exist", "existing");
    }

    [Test]
    public async Task Test_Run_ShouldAddBadgeAndReplyWithSuccessMessage_WhenArgumentsAreValid()
    {
        // Arrange
        _context.Target.Returns("newBadge, image");
        _context.RoomId.Returns("roomId");
        _badgeRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).ReturnsNull();

        // Act
        await _command.Run(_context);

        // Assert
        await _badgeRepository.Received().AddAsync(Arg.Is<Badge>(b =>
            b.Name == "newBadge" &&
            b.Image == "image" &&
            b.Id == "newbadge" &&
            b.IsTrophy == false &&
            b.RoomId == "roomId"));
        _context.Received().ReplyLocalizedMessage("badge_add_success_message");
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithFailureMessage_WhenExceptionThrownByRepository()
    {
        // Arrange
        _context.Target.Returns("newBadge, image");
        _context.RoomId.Returns("roomId");
        _badgeRepository.GetByIdAsync(Arg.Any<Tuple<string, string>>()).ReturnsNull();
        _badgeRepository.AddAsync(Arg.Any<Badge>()).Throws(new Exception("Some error"));

        // Act
        await _command.Run(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("badge_add_failure_message", "Some error");
    }
}