using System.Globalization;
using ElsaMina.Commands.Profile;
using ElsaMina.Commands.Showdown.Ranking;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Formats;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Services.UserData;
using ElsaMina.Core.Services.UserDetails;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Profile;

public class ProfileCommandTest
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

    private ProfileCommand CreateSut(
        IBotDbContextFactory factory,
        out IUserDetailsManager details,
        out ITemplatesManager templates,
        out IUserDataService userData,
        out IShowdownRanksProvider ranks,
        out IFormatsManager formats)
    {
        details = Substitute.For<IUserDetailsManager>();
        templates = Substitute.For<ITemplatesManager>();
        userData = Substitute.For<IUserDataService>();
        ranks = Substitute.For<IShowdownRanksProvider>();
        formats = Substitute.For<IFormatsManager>();

        return new ProfileCommand(details, templates, userData, ranks, formats, factory);
    }

    [Test]
    public async Task RunAsync_ShouldUseSenderUserId_WhenTargetIsEmpty()
    {
        // Arrange
        var options = CreateOptions();
        await using var ctx = new BotDbContext(options);
        await ctx.Database.EnsureCreatedAsync();
        var factory = CreateFactoryReturning(ctx);

        var sut = CreateSut(factory, out var details, out var templates, out var userData, out var ranks,
            out var formats);

        var context = Substitute.For<IContext>();
        context.Target.Returns("");
        context.Sender.UserId.Returns("alice");
        context.RoomId.Returns("room1");
        context.Room.Culture.Returns(new CultureInfo("en-US"));

        details.GetUserDetailsAsync("alice", Arg.Any<CancellationToken>())
            .Returns(new UserDetailsDto { Name = "Alice", Avatar = "5" });

        templates.GetTemplateAsync("Profile/Profile", Arg.Any<object>())
            .Returns("rendered");

        // Act
        await sut.RunAsync(context);

        // Assert
        context.Received().ReplyHtml("rendered", rankAware: true);
    }

    [Test]
    public async Task RunAsync_ShouldUseTarget_WhenProvided()
    {
        // Arrange
        var options = CreateOptions();
        await using var ctx = new BotDbContext(options);
        await ctx.Database.EnsureCreatedAsync();
        var factory = CreateFactoryReturning(ctx);

        var sut = CreateSut(factory, out var details, out var templates, out var userData, out var ranks,
            out var formats);

        var context = Substitute.For<IContext>();
        context.Target.Returns("Bob99");
        context.RoomId.Returns("room1");
        context.Room.Culture.Returns(new CultureInfo("en-US"));

        details.GetUserDetailsAsync("bob99", Arg.Any<CancellationToken>())
            .Returns(new UserDetailsDto { Name = "Bob", Avatar = "7" });

        templates.GetTemplateAsync("Profile/Profile", Arg.Any<object>())
            .Returns("rendered");

        // Act
        await sut.RunAsync(context);

        // Assert
        await details.Received().GetUserDetailsAsync("bob99", Arg.Any<CancellationToken>());
        context.Received().ReplyHtml("rendered", rankAware: true);
    }

    [Test]
    public async Task RunAsync_ShouldUseStoredAvatar_WhenPresent()
    {
        // Arrange
        var options = CreateOptions();
        await using (var setup = new BotDbContext(options))
        {
            await setup.Database.EnsureCreatedAsync();
            setup.RoomUsers.Add(new RoomUser
            {
                Id = "alice",
                RoomId = "room1",
                Avatar = "https://custom/avatar.png"
            });
            await setup.SaveChangesAsync();
        }

        await using var execCtx = new BotDbContext(options);
        var factory = CreateFactoryReturning(execCtx);

        var sut = CreateSut(factory, out var details, out var templates, out var userData, out var ranks,
            out var formats);

        details.GetUserDetailsAsync("alice", Arg.Any<CancellationToken>())
            .Returns(new UserDetailsDto { Name = "Alice", Avatar = "3" });

        templates.GetTemplateAsync("Profile/Profile", Arg.Any<object>())
            .Returns("rendered");

        var context = Substitute.For<IContext>();
        context.Target.Returns("alice");
        context.RoomId.Returns("room1");
        context.Room.Culture.Returns(new CultureInfo("en-US"));

        // Act
        await sut.RunAsync(context);

        // Assert
        await templates.Received().GetTemplateAsync("Profile/Profile",
            Arg.Is<ProfileViewModel>(vm =>
                vm.Avatar == "https://custom/avatar.png"
            )
        );
    }

    [Test]
    public async Task RunAsync_ShouldCleanFormatId_WhenBestRankingPresent()
    {
        // Arrange
        var options = CreateOptions();
        await using var ctx = new BotDbContext(options);
        await ctx.Database.EnsureCreatedAsync();

        var factory = CreateFactoryReturning(ctx);
        var sut = CreateSut(factory, out var details, out var templates, out var userData, out var ranks,
            out var formats);

        details.GetUserDetailsAsync("alice", Arg.Any<CancellationToken>())
            .Returns(new UserDetailsDto());

        ranks.GetRankingDataAsync("alice", Arg.Any<CancellationToken>())
            .Returns(new[]
            {
                new RankingDataDto { FormatId = "gen7ou", Elo = 1500 },
                new RankingDataDto { FormatId = "gen8ou", Elo = 1600 }
            });

        formats.GetCleanFormat("gen8ou").Returns("ou");

        templates.GetTemplateAsync("Profile/Profile", Arg.Any<object>())
            .Returns("rendered");

        var context = Substitute.For<IContext>();
        context.Target.Returns("alice");
        context.RoomId.Returns("room1");
        context.Room.Culture.Returns(new CultureInfo("en-US"));

        // Act
        await sut.RunAsync(context);

        // Assert
        await templates.Received().GetTemplateAsync("Profile/Profile",
            Arg.Is<ProfileViewModel>(vm =>
                vm.BestRanking.FormatId == "ou"
            )
        );
    }

    [Test]
    public async Task RunAsync_ShouldReplyHtml()
    {
        // Arrange
        var options = CreateOptions();
        await using var ctx = new BotDbContext(options);
        await ctx.Database.EnsureCreatedAsync();
        var factory = CreateFactoryReturning(ctx);

        var sut = CreateSut(factory, out var details, out var templates, out var userData, out var ranks,
            out var formats);

        details.GetUserDetailsAsync("john", Arg.Any<CancellationToken>())
            .Returns(new UserDetailsDto { Name = "John" });

        templates.GetTemplateAsync("Profile/Profile", Arg.Any<object>())
            .Returns("rendered");

        var context = Substitute.For<IContext>();
        context.Target.Returns("john");
        context.RoomId.Returns("room1");
        context.Room.Culture.Returns(new CultureInfo("en-US"));

        // Act
        await sut.RunAsync(context);

        // Assert
        context.Received().ReplyHtml("rendered", rankAware: true);
    }
       [Test]
    public async Task RunAsync_ShouldLoadBadges_WhenStoredUserDataHasBadges()
    {
        // Arrange
        var options = CreateOptions();
        await using (var setup = new BotDbContext(options))
        {
            await setup.Database.EnsureCreatedAsync();
            var badge = new Badge { Id = "b1", Name = "TestBadge", RoomId = "room1"};
            setup.Badges.Add(badge);
            setup.RoomUsers.Add(new RoomUser
            {
                Id = "alice",
                RoomId = "room1",
                Badges = new List<BadgeHolding>
                {
                    new() { Badge = badge }
                }
            });
            await setup.SaveChangesAsync();
        }

        await using var execCtx = new BotDbContext(options);
        var factory = CreateFactoryReturning(execCtx);
        var sut = CreateSut(factory, out var details, out var templates, out var userData, out var ranks, out var formats);

        details.GetUserDetailsAsync("alice", Arg.Any<CancellationToken>())
               .Returns(new UserDetailsDto { Name = "Alice" });

        templates.GetTemplateAsync("Profile/Profile", Arg.Any<object>())
                 .Returns("rendered");

        var context = Substitute.For<IContext>();
        context.Target.Returns("alice");
        context.RoomId.Returns("room1");
        context.Room.Culture.Returns(new CultureInfo("en-US"));

        // Act
        await sut.RunAsync(context);

        // Assert
        await templates.Received().GetTemplateAsync("Profile/Profile",
            Arg.Is<ProfileViewModel>(vm =>
                vm.Badges != null && vm.Badges.Any() && vm.Badges.First().Name == "TestBadge"
            )
        );
    }

    [TestCase("!busy", "busy")]
    [TestCase("online", "online")]
    [TestCase(null, null)]
    public async Task RunAsync_ShouldParseStatusCorrectly(string inputStatus, string expected)
    {
        // Arrange
        var options = CreateOptions();
        await using var ctx = new BotDbContext(options);
        await ctx.Database.EnsureCreatedAsync();
        var factory = CreateFactoryReturning(ctx);

        var sut = CreateSut(factory, out var details, out var templates, out var userData, out var ranks, out var formats);

        details.GetUserDetailsAsync("bob", Arg.Any<CancellationToken>())
               .Returns(new UserDetailsDto { Status = inputStatus });

        templates.GetTemplateAsync("Profile/Profile", Arg.Any<object>())
                 .Returns("rendered");

        var context = Substitute.For<IContext>();
        context.Target.Returns("bob");
        context.RoomId.Returns("room1");
        context.Room.Culture.Returns(new CultureInfo("en-US"));

        // Act
        await sut.RunAsync(context);

        // Assert
        await templates.Received().GetTemplateAsync("Profile/Profile",
            Arg.Is<ProfileViewModel>(vm =>
                vm.Status == expected
            )
        );
    }

    [Test]
    public void GetAvatar_ShouldReturnDefaultAvatar_WhenNoStoredOrShowdownAvatar()
    {
        // Act
        var avatar = ProfileCommand.GetAvatar(null, null);

        // Assert
        Assert.That(avatar, Is.EqualTo("https://play.pokemonshowdown.com/sprites/trainers/unknown.png"));
    }

    [Test]
    public void GetAvatar_ShouldReturnCustomStoredAvatar_WhenPresent()
    {
        // Arrange
        var stored = new RoomUser { Avatar = "https://custom/avatar.png" };

        // Act
        var avatar = ProfileCommand.GetAvatar(stored, null);

        // Assert
        Assert.That(avatar, Is.EqualTo("https://custom/avatar.png"));
    }

    [Test]
    public void GetAvatar_ShouldReturnCustomUrl_WhenAvatarStartsWithHash()
    {
        // Arrange
        var details = new UserDetailsDto { Avatar = "#123" };

        // Act
        var avatar = ProfileCommand.GetAvatar(null, details);

        // Assert
        Assert.That(avatar.Contains("trainers-custom/123.png"), Is.True);
    }

    [Test]
    public async Task RunAsync_ShouldHandleNullStoredAndShowdownUserData()
    {
        // Arrange
        var options = CreateOptions();
        await using var ctx = new BotDbContext(options);
        await ctx.Database.EnsureCreatedAsync();
        var factory = CreateFactoryReturning(ctx);

        var sut = CreateSut(factory, out var details, out var templates, out var userData, out var ranks, out var formats);

        details.GetUserDetailsAsync("ghost", Arg.Any<CancellationToken>())
               .Returns((UserDetailsDto)null);

        templates.GetTemplateAsync("Profile/Profile", Arg.Any<object>())
                 .Returns("rendered");

        var context = Substitute.For<IContext>();
        context.Target.Returns("ghost");
        context.RoomId.Returns("room1");
        context.Room.Culture.Returns(new CultureInfo("en-US"));

        // Act
        await sut.RunAsync(context);

        // Assert
        await templates.Received().GetTemplateAsync("Profile/Profile",
            Arg.Is<ProfileViewModel>(vm => vm.UserName == "ghost")
        );
    }

    [Test]
    public async Task RunAsync_ShouldExtractUserRoomRankCorrectly()
    {
        // Arrange
        var options = CreateOptions();
        await using var ctx = new BotDbContext(options);
        await ctx.Database.EnsureCreatedAsync();
        var factory = CreateFactoryReturning(ctx);

        var sut = CreateSut(factory, out var details, out var templates, out var userData, out var ranks, out var formats);

        details.GetUserDetailsAsync("alice", Arg.Any<CancellationToken>())
               .Returns(new UserDetailsDto
               {
                   Rooms = new Dictionary<string, UserDetailsRoomDto> { { "+room1", null } }
               });

        templates.GetTemplateAsync("Profile/Profile", Arg.Any<object>())
                 .Returns("rendered");

        var context = Substitute.For<IContext>();
        context.Target.Returns("alice");
        context.RoomId.Returns("room1");
        context.Room.Culture.Returns(new CultureInfo("en-US"));

        // Act
        await sut.RunAsync(context);

        // Assert
        await templates.Received().GetTemplateAsync("Profile/Profile",
            Arg.Is<ProfileViewModel>(vm => vm.UserRoomRank == '+')
        );
    }
}