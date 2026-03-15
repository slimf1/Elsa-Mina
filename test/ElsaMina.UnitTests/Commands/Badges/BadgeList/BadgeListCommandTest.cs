using System.Globalization;
using ElsaMina.Commands.Badges.BadgeList;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Badges.BadgeList;

[TestFixture]
public class BadgeListCommandTest
{
    private IContext _context;
    private IRoomsManager _roomsManager;
    private IBotDbContextFactory _dbContextFactory;
    private ITemplatesManager _templatesManager;
    private BotDbContext _dbContext;
    private BadgeListCommand _command;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new BotDbContext(options);

        _context = Substitute.For<IContext>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _templatesManager = Substitute.For<ITemplatesManager>();

        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(_dbContext));

        _command = new BadgeListCommand(_roomsManager, _dbContextFactory, _templatesManager);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public void Test_RequiredRank_ShouldBeVoiced()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Voiced));
    }

    [Test]
    public void Test_IsAllowedInPrivateMessage_ShouldBeTrue()
    {
        Assert.That(_command.IsAllowedInPrivateMessage, Is.True);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyLocalizedMessage_WhenRoomNotFound()
    {
        // Arrange
        _context.Target.Returns(string.Empty);
        _context.RoomId.Returns("unknownroom");
        _roomsManager.HasRoom("unknownroom").Returns(false);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badgelist_room_not_found", "unknownroom");
        await _templatesManager.DidNotReceive().GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyLocalizedMessage_WhenTargetRoomNotFound()
    {
        // Arrange
        _context.Target.Returns("nonexistentroom");
        _roomsManager.HasRoom("nonexistentroom").Returns(false);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("badgelist_room_not_found", "nonexistentroom");
    }

    [Test]
    public async Task Test_RunAsync_ShouldRenderTemplate_WhenRoomExistsAndHasBadges()
    {
        // Arrange
        var roomId = "room1";
        _context.Target.Returns(string.Empty);
        _context.RoomId.Returns(roomId);
        _context.Culture.Returns(CultureInfo.InvariantCulture);
        _roomsManager.HasRoom(roomId).Returns(true);

        await _dbContext.Badges.AddRangeAsync(
            new Badge { Id = "b1", Name = "Zebra Badge", RoomId = roomId },
            new Badge { Id = "b2", Name = "Apple Badge", RoomId = roomId }
        );
        await _dbContext.SaveChangesAsync();

        var expectedHtml = "<div>badge list</div>";
        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>()).Returns(expectedHtml);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Badges/BadgeList/BadgeList",
            Arg.Is<BadgeListViewModel>(vm =>
                vm.Badges.Length == 2 &&
                vm.Badges[0].Name == "Apple Badge" &&
                vm.Badges[1].Name == "Zebra Badge" &&
                vm.Culture == CultureInfo.InvariantCulture
            ));
        _context.Received(1).ReplyHtml(expectedHtml, rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldFilterBadgesByRoom()
    {
        // Arrange
        var roomId = "room1";
        _context.Target.Returns(string.Empty);
        _context.RoomId.Returns(roomId);
        _context.Culture.Returns(CultureInfo.InvariantCulture);
        _roomsManager.HasRoom(roomId).Returns(true);

        await _dbContext.Badges.AddRangeAsync(
            new Badge { Id = "b1", Name = "Room Badge", RoomId = roomId },
            new Badge { Id = "b2", Name = "Other Badge", RoomId = "otherroom" }
        );
        await _dbContext.SaveChangesAsync();

        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>()).Returns("html");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Is<BadgeListViewModel>(vm => vm.Badges.Length == 1 && vm.Badges[0].RoomId == roomId)
        );
    }

    [Test]
    public async Task Test_RunAsync_ShouldUsTargetAsRoomId_WhenTargetIsProvided()
    {
        // Arrange
        var targetRoomId = "targetroom";
        _context.Target.Returns(targetRoomId);
        _context.RoomId.Returns("currentroom");
        _context.Culture.Returns(CultureInfo.InvariantCulture);
        _roomsManager.HasRoom(targetRoomId).Returns(true);

        await _dbContext.Badges.AddAsync(new Badge { Id = "b1", Name = "Badge", RoomId = targetRoomId });
        await _dbContext.SaveChangesAsync();

        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>()).Returns("html");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Is<BadgeListViewModel>(vm => vm.Badges.Length == 1 && vm.Badges[0].RoomId == targetRoomId)
        );
    }
}