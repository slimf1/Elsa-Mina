using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.IntegrationTests.DataAccess.Repositories;

public class SavedPollRepositoryTests
{
    private DbContextOptions<BotDbContext> _dbOptions;
    private BotDbContext _dbContext;
    private SavedPollRepository _repository;

    [SetUp]
    public void Setup()
    {
        _dbOptions = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new BotDbContext(_dbOptions);
        _repository = new SavedPollRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Dispose();
        _dbContext.Dispose();
    }

    [Test]
    public async Task Test_AddAsync_ShouldAddPoll()
    {
        // Arrange
        var roomInfo = new RoomInfo { Id = "room1" };
        var poll = new SavedPoll
        {
            Id = 1,
            RoomId = "room1",
            RoomInfo = roomInfo,
            Content = "Poll content",
            EndedAt = DateTimeOffset.UnixEpoch
        };

        // Act
        await _dbContext.RoomInfo.AddAsync(roomInfo);
        await _repository.AddAsync(poll);
        var saved = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved.RoomId, Is.EqualTo("room1"));
    }

    [Test]
    public async Task Test_UpdateAsync_ShouldUpdatePollContent()
    {
        // Arrange
        var poll = new SavedPoll
        {
            Id = 1,
            RoomId = "room1",
            RoomInfo = new RoomInfo { Id = "room1" },
            Content = "Old content",
            EndedAt = DateTimeOffset.UnixEpoch
        };
        await _repository.AddAsync(poll);

        // Act
        poll.Content = "Updated content";
        await _repository.UpdateAsync(poll);
        var updated = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(updated.Content, Is.EqualTo("Updated content"));
    }

    [Test]
    public async Task Test_DeleteAsync_ShouldRemovePoll()
    {
        // Arrange
        var poll = new SavedPoll
        {
            Id = 1,
            RoomId = "room1",
            RoomInfo = new RoomInfo { Id = "room1" },
            Content = "Poll content",
            EndedAt = DateTimeOffset.UtcNow
        };
        await _repository.AddAsync(poll);

        // Act
        await _repository.DeleteAsync(poll);
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Test_GetByIdAsync_ShouldIncludeRoomInfo()
    {
        // Arrange
        var poll = new SavedPoll
        {
            Id = 1,
            RoomId = "room1",
            RoomInfo = new RoomInfo { Id = "room1" },
            Content = "Poll content",
            EndedAt = DateTimeOffset.UtcNow
        };
        await _repository.AddAsync(poll);

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.RoomInfo, Is.Not.Null);
        Assert.That(result.RoomInfo.Id, Is.EqualTo("room1"));
    }

    [Test]
    public async Task Test_GetAllAsync_ShouldReturnAllPolls()
    {
        // Arrange
        await _repository.AddAsync(new SavedPoll { Id = 1, RoomId = "room1", RoomInfo = new RoomInfo { Id = "room1" }, Content = "Poll 1", EndedAt = DateTimeOffset.UtcNow });
        await _repository.AddAsync(new SavedPoll { Id = 2, RoomId = "room2", RoomInfo = new RoomInfo { Id = "room2" }, Content = "Poll 2", EndedAt = DateTimeOffset.UtcNow });

        // Act
        var all = (await _repository.GetAllAsync()).ToList();

        // Assert
        Assert.That(all, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Test_GetPollsByRoomIdAsync_ShouldReturnMatchingPolls()
    {
        // Arrange
        await _repository.AddAsync(new SavedPoll { Id = 1, RoomId = "room1", RoomInfo = new RoomInfo { Id = "room1" }, Content = "Poll A", EndedAt = DateTimeOffset.UtcNow });
        await _repository.AddAsync(new SavedPoll { Id = 2, RoomId = "room2", RoomInfo = new RoomInfo { Id = "room2" }, Content = "Poll B", EndedAt = DateTimeOffset.UtcNow });

        // Act
        var result = (await _repository.GetPollsByRoomIdAsync("room1")).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].RoomId, Is.EqualTo("room1"));
    }
}
