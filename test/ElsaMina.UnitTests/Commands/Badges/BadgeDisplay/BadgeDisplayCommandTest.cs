using System.Globalization;
using ElsaMina.Commands.Badges.BadgeDisplay;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Badges.BadgeDisplay;

[TestFixture]
public class BadgeDisplayCommandTests
{
    private BadgeDisplayCommand _command;
    private IBotDbContextFactory _dbContextFactory;
    private ITemplatesManager _templatesManager;
    private BotDbContext _dbContext;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        // Arrange
        // Setup In-Memory Database
        var options = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name per test to ensure isolation
            .Options;
        _dbContext = new BotDbContext(options);

        // Mock Factory to return the in-memory context
        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(_dbContext));

        _templatesManager = Substitute.For<ITemplatesManager>();
        _context = Substitute.For<IContext>();

        _command = new BadgeDisplayCommand(_dbContextFactory, _templatesManager);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyLocalizedMessage_WhenBadgeNotFound()
    {
        // Arrange
        var targetBadge = "nonexistentbadge";
        _context.Target.Returns(targetBadge);
        _context.RoomId.Returns("room-1");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badge_display_not_found", targetBadge);
        await _templatesManager.DidNotReceive().GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>());
        _context.DidNotReceive().ReplyHtml(Arg.Any<string>(), rankAware: Arg.Any<bool>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyHtml_WhenBadgeExists()
    {
        // Arrange
        var roomId = "room-1";
        var badgeId = "testbadge";
        var targetInput = "TestBadge"; // Assuming ToLowerAlphaNum converts this to "testbadge"
        var expectedHtml = "<div>Badge Content</div>";

        // Seed Database
        var badge = new Badge
        {
            Id = badgeId,
            Name = "Test Badge",
            RoomId = roomId,
            BadgeHolders = new List<BadgeHolding>
            {
                new()
                {
                    UserId = "user1",
                    RoomUser = new RoomUser
                    {
                        RoomId = roomId,
                        User = new SavedUser { UserId = "userone", UserName = "UserOne" }
                    }
                },
                new()
                {
                    UserId = "user2",
                    // Simulating a user not currently in the room (RoomUser is null)
                    RoomUser = null
                }
            }
        };

        _dbContext.Badges.Add(badge);
        await _dbContext.SaveChangesAsync();

        _context.Target.Returns(targetInput);
        _context.RoomId.Returns(roomId);
        _context.Culture.Returns(CultureInfo.InvariantCulture);

        _templatesManager.GetTemplateAsync("Badges/BadgeDisplay/BadgeDisplay", Arg.Any<BadgeDisplayViewModel>())
            .Returns(Task.FromResult(expectedHtml));

        // Act
        await _command.RunAsync(_context);

        // Assert
        // Verify Template Manager was called with correct ViewModel
        await _templatesManager.Received(1).GetTemplateAsync(
            "Badges/BadgeDisplay/BadgeDisplay",
            Arg.Is<BadgeDisplayViewModel>(vm =>
                vm.DisplayedBadge.Id == badgeId &&
                vm.BadgeHolders.Length == 2 &&
                vm.BadgeHolders.Contains("UserOne") &&
                vm.BadgeHolders.Contains("user2") && // Fallback to UserId when RoomUser is null
                vm.Culture == CultureInfo.InvariantCulture
            ));

        // Verify HTML reply was sent
        _context.Received(1).ReplyHtml(Arg.Is<string>(s => s == expectedHtml), rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldIgnoreBadge_WhenRoomIdDoesNotMatch()
    {
        // Arrange
        var badgeId = "globalbadge";
        var badgeRoomId = "room-A";
        var contextRoomId = "room-B";

        // Seed Database with a badge in a different room
        var badge = new Badge
        {
            Id = badgeId,
            RoomId = badgeRoomId
        };

        _dbContext.Badges.Add(badge);
        await _dbContext.SaveChangesAsync();

        _context.Target.Returns(badgeId);
        _context.RoomId.Returns(contextRoomId); // Context is in a different room

        // Act
        await _command.RunAsync(_context);

        // Assert
        // Should be treated as not found because the room ID doesn't match
        _context.Received(1).ReplyLocalizedMessage("badge_display_not_found", badgeId);
        _context.DidNotReceive().ReplyHtml(Arg.Any<string>(), rankAware: Arg.Any<bool>());
    }
}