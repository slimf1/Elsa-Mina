using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ElsaMina.IntegrationTests.DataAccess.Repositories;

public class AddedCommandRepositoryTests
{
    private DbContextOptions<BotDbContext> _dbOptions;
    private BotDbContext _dbContext;
    private AddedCommandRepository _repository;

    [SetUp]
    public void Setup()
    {
        _dbOptions = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new BotDbContext(_dbOptions);
        _repository = new AddedCommandRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _repository.Dispose();
        _dbContext.Dispose();
    }

    [Test]
    public async Task Test_AddAsync_ShouldAddCommand()
    {
        // Arrange
        var command = new AddedCommand
        {
            Id = "cmd1",
            RoomId = "room1",
            Content = "Hello there!"
        };

        // Act
        await _repository.AddAsync(command);
        var saved = await _repository.GetByIdAsync(Tuple.Create("cmd1", "room1"));

        // Assert
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved.Content, Is.EqualTo("Hello there!"));
    }

    [Test]
    public async Task Test_UpdateAsync_ShouldUpdateCommand()
    {
        // Arrange
        var command = new AddedCommand { Id = "cmd1", RoomId = "room1", Content = "Hi" };
        await _repository.AddAsync(command);

        // Act
        command.Content = "Updated";
        await _repository.UpdateAsync(command);
        var updated = await _repository.GetByIdAsync(Tuple.Create("cmd1", "room1"));

        // Assert
        Assert.That(updated.Content, Is.EqualTo("Updated"));
    }

    [Test]
    public async Task Test_DeleteAsync_ShouldRemoveCommand()
    {
        // Arrange
        var command = new AddedCommand { Id = "cmd1", RoomId = "room1", Content = "To remove" };
        await _repository.AddAsync(command);

        // Act
        await _repository.DeleteAsync(command);
        var result = await _repository.GetByIdAsync(Tuple.Create("cmd1", "room1"));

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Test_GetByIdAsync_ShouldReturnCorrectCommand()
    {
        // Arrange
        var command = new AddedCommand { Id = "cmd1", RoomId = "room1", Content = "Exact match" };
        await _repository.AddAsync(command);

        // Act
        var result = await _repository.GetByIdAsync(Tuple.Create("cmd1", "room1"));

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo("cmd1"));
        Assert.That(result.RoomId, Is.EqualTo("room1"));
    }

    [Test]
    public async Task Test_GetAllAsync_ShouldReturnAllCommands()
    {
        // Arrange
        await _repository.AddAsync(new AddedCommand { Id = "cmd1", RoomId = "room1", Content = "Resp 1" });
        await _repository.AddAsync(new AddedCommand { Id = "cmd2", RoomId = "room2", Content = "Resp 2" });

        // Act
        var all = (await _repository.GetAllAsync()).ToList();

        // Assert
        Assert.That(all, Has.Count.EqualTo(2));
    }
}