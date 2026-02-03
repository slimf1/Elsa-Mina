using ElsaMina.Commands.Badges;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Badges;

public class EditBadgeCommandTest
{
    private DbContextOptions<BotDbContext> CreateOptions() =>
        new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    private IBotDbContextFactory CreateFactoryReturning(BotDbContext ctx)
    {
        var factory = Substitute.For<IBotDbContextFactory>();
        factory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(ctx);
        return factory;
    }

    [Test]
    public void RequiredRank_ShouldBeDriver()
    {
        // Act
        var command = new EditBadgeCommand(Substitute.For<IBotDbContextFactory>());

        // Assert
        Assert.That(command.RequiredRank, Is.EqualTo(Rank.Driver));
    }

    [Test]
    public void RunAsync_ShouldReplyHelp_WhenArgumentsAreMissing()
    {
        // Act
        var command = new EditBadgeCommand(Substitute.For<IBotDbContextFactory>());
        
        // Assert
        Assert.That(command.HelpMessageKey, Is.EqualTo("badge_edit_help_message"));
    }

    [Test]
    public async Task RunAsync_ShouldUpdateBadge_WhenBadgeExistsInCurrentRoom()
    {
        // Arrange
        var options = CreateOptions();
        await using (var setup = new BotDbContext(options))
        {
            await setup.Database.EnsureCreatedAsync();
            setup.Badges.Add(new Badge
            {
                Id = "badge1",
                RoomId = "room1",
                Name = "Old Name",
                Image = "old.png"
            });
            await setup.SaveChangesAsync();
        }

        await using var execCtx = new BotDbContext(options);
        var factory = CreateFactoryReturning(execCtx);
        var context = Substitute.For<IContext>();
        context.Target.Returns("badge1, New Name, https://img.test/new.png");
        context.RoomId.Returns("room1");

        var command = new EditBadgeCommand(factory);

        // Act
        await command.RunAsync(context);

        // Assert
        await using (var assertCtx = new BotDbContext(options))
        {
            var badge = await assertCtx.Badges.FindAsync("badge1", "room1");
            Assert.That(badge.Name, Is.EqualTo("New Name"));
            Assert.That(badge.Image, Is.EqualTo("https://img.test/new.png"));
        }

        context.Received().ReplyLocalizedMessage("badge_edit_success", "badge1");
    }

    [Test]
    public async Task RunAsync_ShouldUseRoomIdFromArguments_WhenProvided()
    {
        // Arrange
        var options = CreateOptions();
        await using (var setup = new BotDbContext(options))
        {
            await setup.Database.EnsureCreatedAsync();
            setup.Badges.Add(new Badge
            {
                Id = "badge1",
                RoomId = "room2",
                Name = "Old Name",
                Image = "old.png"
            });
            setup.Badges.Add(new Badge
            {
                Id = "badge1",
                RoomId = "room1",
                Name = "Room1 Name",
                Image = "room1.png"
            });
            await setup.SaveChangesAsync();
        }

        await using var execCtx = new BotDbContext(options);
        var factory = CreateFactoryReturning(execCtx);
        var context = Substitute.For<IContext>();
        context.Target.Returns("badge1, New Name, https://img.test/new.png, room2");
        context.RoomId.Returns("room1");

        var command = new EditBadgeCommand(factory);

        // Act
        await command.RunAsync(context);

        // Assert
        await using (var assertCtx = new BotDbContext(options))
        {
            var room2Badge = await assertCtx.Badges.FindAsync("badge1", "room2");
            var room1Badge = await assertCtx.Badges.FindAsync("badge1", "room1");
            Assert.That(room2Badge.Name, Is.EqualTo("New Name"));
            Assert.That(room2Badge.Image, Is.EqualTo("https://img.test/new.png"));
            Assert.That(room1Badge.Name, Is.EqualTo("Room1 Name"));
            Assert.That(room1Badge.Image, Is.EqualTo("room1.png"));
        }
    }

    [Test]
    public async Task RunAsync_ShouldReplyNotFound_WhenBadgeDoesNotExist()
    {
        // Arrange
        var options = CreateOptions();
        await using var ctx = new BotDbContext(options);
        await ctx.Database.EnsureCreatedAsync();

        var factory = CreateFactoryReturning(ctx);
        var context = Substitute.For<IContext>();
        context.Target.Returns("missing, New Name, img.png");
        context.RoomId.Returns("room1");

        var command = new EditBadgeCommand(factory);

        // Act
        await command.RunAsync(context);

        // Assert
        context.Received().ReplyLocalizedMessage("badge_edit_not_found", "missing");
    }
}
