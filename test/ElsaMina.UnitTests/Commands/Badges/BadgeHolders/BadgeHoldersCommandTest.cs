using ElsaMina.Commands.Badges.BadgeHolders;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;

namespace ElsaMina.UnitTests.Commands.Badges.BadgeHolders;

[TestFixture]
public class BadgeHoldersCommandTest
{
    private IContext _context;
    private ITemplatesManager _templatesManager;
    private IBotDbContextFactory _dbContextFactory;
    private BadgeHoldersCommand _command;

    private DbContextOptions<BotDbContext> CreateNewInMemoryOptions()
    {
        return new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _command = new BadgeHoldersCommand(_templatesManager, _dbContextFactory);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeRegular()
    {
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Regular));
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisplayBadges_WhenBadgesExist()
    {
        // Arrange
        var options = CreateNewInMemoryOptions();
        await using var dbContext = new BotDbContext(options);

        var badge1 = new Badge
        {
            Id = "badge1",
            Name = "Badge A",
            RoomId = "room1",
            IsTrophy = false
        };
        var badge2 = new Badge
        {
            Id = "badge2",
            Name = "Badge B",
            RoomId = "room1",
            IsTrophy = false
        };

        badge1.BadgeHolders.Add(new BadgeHolding { UserId = "user1", BadgeId = badge1.Id, RoomId = badge1.RoomId });

        await dbContext.Badges.AddRangeAsync(badge1, badge2);
        await dbContext.SaveChangesAsync();

        _dbContextFactory.CreateDbContextAsync(default).Returns(dbContext);

        _context.RoomId.Returns("room1");
        _context.Culture.Returns(System.Globalization.CultureInfo.InvariantCulture);

        var expectedHtml = "<div>Badge Holders</div>";
        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>()).Returns(expectedHtml);

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync("Badges/BadgeHolders/BadgeHolders", Arg.Is<BadgeHoldersViewModel>(vm =>
            vm.Badges.Length == 2 &&
            vm.Badges[0].Id == "badge1" &&
            vm.Badges[0].BadgeHolders.Count == 1 &&
            vm.Badges[1].Id == "badge2"
        ));

        _context.Received(1).ReplyHtml(expectedHtml, null, true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldFilterByRoom_AndSortByName()
    {
        // Arrange
        var options = CreateNewInMemoryOptions();
        await using var dbContext = new BotDbContext(options);

        var badgeInRoomB = new Badge { Id = "b1", Name = "Zebra Badge", RoomId = "room1" };
        var badgeInRoomA = new Badge { Id = "b2", Name = "Apple Badge", RoomId = "room1" };
        var badgeOtherRoom = new Badge { Id = "b3", Name = "Other Room Badge", RoomId = "other_room" };

        await dbContext.Badges.AddRangeAsync(badgeInRoomB, badgeInRoomA, badgeOtherRoom);
        await dbContext.SaveChangesAsync();

        _dbContextFactory.CreateDbContextAsync(default).Returns(dbContext);

        _context.RoomId.Returns("room1");

        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>()).Returns("html");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(Arg.Any<string>(), Arg.Is<BadgeHoldersViewModel>(vm =>
            vm.Badges.Length == 2 &&
            vm.Badges[0].Name == "Apple Badge" && // Sorted by Name
            vm.Badges[1].Name == "Zebra Badge"
        ));
    }
}