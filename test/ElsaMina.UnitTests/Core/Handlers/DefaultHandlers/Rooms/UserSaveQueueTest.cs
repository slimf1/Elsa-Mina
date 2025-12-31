using ElsaMina.Core.Handlers.DefaultHandlers.Rooms;
using ElsaMina.Core.Services.Config;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ElsaMina.UnitTests.Core.Handlers.DefaultHandlers.Rooms;

public class UserSaveQueueTests
{
    private IConfiguration _configuration = null!;
    private IBotDbContextFactory _dbContextFactory = null!;
    private DbContextOptions<BotDbContext> _options = null!;

    [SetUp]
    public void SetUp()
    {
        _options = CreateInMemoryOptions();

        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContextFactory
            .CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(new BotDbContext(_options)));

        _configuration = Substitute.For<IConfiguration>();
        _configuration.UserUpdateBatchSize.Returns(2);
        _configuration.UserUpdateFlushInterval.Returns(TimeSpan.FromMilliseconds(50));
    }

    [Test]
    public async Task Enqueue_ShouldFlush_WhenBatchSizeReached()
    {
        // Arrange
        var queue = CreateQueue();

        // Act
        queue.Enqueue("AUser");
        queue.Enqueue("BUser"); // déclenche le flush
        await queue.WaitForFlushAsync();

        // Assert
        var users = await LoadUsersAsync();

        Assert.That(users, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(users.Select(u => u.UserId),
                Is.EquivalentTo((string[]) ["auser", "buser"]));
        });
    }

    [Test]
    public async Task FlushAsync_ShouldUpdateExistingUser_WhenUserAlreadyExists()
    {
        // Arrange
        await SeedUserAsync(new SavedUser
        {
            UserId = "user1",
            UserName = "OldName"
        });

        var queue = CreateQueue();
        queue.Enqueue(" User 1");

        // Act
        await queue.FlushAsync(CancellationToken.None);

        // Assert
        await using var db = new BotDbContext(_options);
        var user = await db.Users.FindAsync("user1");

        Assert.That(user, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(user!.UserId, Is.EqualTo("user1"));
            Assert.That(user.UserName, Is.EqualTo("User 1"));
        });
    }

    [Test]
    public async Task WaitForFlushAsync_ShouldAwaitFlush_WhenFlushIsInProgress()
    {
        // Arrange
        var tcs = new TaskCompletionSource();

        _dbContextFactory
            .CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(async _ =>
            {
                await Task.Delay(100);
                return new BotDbContext(_options);
            });

        var queue = CreateQueue();
        queue.Enqueue("UserA");
        queue.Enqueue("UserB"); // déclenche le flush

        // Act
        var waitTask = queue.WaitForFlushAsync();

        // Assert
        Assert.That(waitTask.IsCompleted, Is.False);
        await waitTask;
    }

    private UserSaveQueue CreateQueue()
        => new(_dbContextFactory, _configuration);

    private static DbContextOptions<BotDbContext> CreateInMemoryOptions()
        => new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    private async Task SeedUserAsync(SavedUser user)
    {
        await using var db = new BotDbContext(_options);
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }

    private async Task<List<SavedUser>> LoadUsersAsync()
    {
        await using var db = new BotDbContext(_options);
        return await db.Users.AsNoTracking().ToListAsync();
    }
}
