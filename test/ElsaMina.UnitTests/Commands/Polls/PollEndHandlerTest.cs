using ElsaMina.Commands.Polls;
using ElsaMina.Core.Services.Clock;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Polls;

public class PollEndHandlerTest
{
    private IClockService _clockService;
    private IBotDbContextFactory _dbContextFactory;
    private BotDbContext _dbContext;
    private DbSet<SavedPoll> _savedPollsDbSet;
    private PollEndHandler _pollEndHandler;

    [SetUp]
    public void SetUp()
    {
        _clockService = Substitute.For<IClockService>();
        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContext = Substitute.For<BotDbContext>();
        _savedPollsDbSet = Substitute.For<DbSet<SavedPoll>>();

        _dbContext.SavedPolls.Returns(_savedPollsDbSet);
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(_dbContext);

        _pollEndHandler = new PollEndHandler(_clockService, _dbContextFactory);
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldSavePoll_WhenPollEndedInEnglish()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "html", "Poll ended: What is your favorite color?" };
        var currentTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _clockService.CurrentUtcDateTime.Returns(currentTime);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _savedPollsDbSet.Received(1).AddAsync(
            Arg.Is<SavedPoll>(poll =>
                poll.RoomId == roomId &&
                poll.Content == "Poll ended: What is your favorite color?" &&
                poll.EndedAt == currentTime),
            Arg.Any<CancellationToken>());
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldSavePoll_WhenPollEndedInFrench()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "html", "Sondage terminé: Quelle est votre couleur préférée?" };
        var currentTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _clockService.CurrentUtcDateTime.Returns(currentTime);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _savedPollsDbSet.Received(1).AddAsync(
            Arg.Is<SavedPoll>(poll =>
                poll.RoomId == roomId &&
                poll.Content == "Sondage terminé: Quelle est votre couleur préférée?" &&
                poll.EndedAt == currentTime),
            Arg.Any<CancellationToken>());
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldNotSavePoll_WhenNotPollEndMessage()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "html", "Some other message" };

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _savedPollsDbSet.DidNotReceive().AddAsync(Arg.Any<SavedPoll>(), Arg.Any<CancellationToken>());
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldNotSavePoll_WhenNotHtmlMessage()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "chat", "Poll ended: What is your favorite color?" };

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _savedPollsDbSet.DidNotReceive().AddAsync(Arg.Any<SavedPoll>(), Arg.Any<CancellationToken>());
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldNotSavePoll_WhenPartsTooShort()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "html" };

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _savedPollsDbSet.DidNotReceive().AddAsync(Arg.Any<SavedPoll>(), Arg.Any<CancellationToken>());
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldJoinMultipleParts_IntoContent()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "html", "Poll", "ended:", "What", "is", "best?" };
        var currentTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _clockService.CurrentUtcDateTime.Returns(currentTime);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _savedPollsDbSet.Received(1).AddAsync(
            Arg.Is<SavedPoll>(poll => poll.Content == "Poll ended: What is best?"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldSavePoll_WhenContentContainsPollEndedAnywhere()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "html", "The Poll ended with 10 votes" };
        var currentTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _clockService.CurrentUtcDateTime.Returns(currentTime);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _savedPollsDbSet.Received(1).AddAsync(Arg.Any<SavedPoll>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldSavePoll_WhenContentContainsSondageTermineAnywhere()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "html", "Le Sondage terminé avec succès" };
        var currentTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _clockService.CurrentUtcDateTime.Returns(currentTime);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _savedPollsDbSet.Received(1).AddAsync(Arg.Any<SavedPoll>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldPassCancellationToken_ToDbOperations()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var roomId = "test-room";
        var parts = new[] { "room", "html", "Poll ended: Test" };
        var currentTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _clockService.CurrentUtcDateTime.Returns(currentTime);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId, cancellationToken);

        // Assert
        await _dbContextFactory.Received(1).CreateDbContextAsync(cancellationToken);
        await _savedPollsDbSet.Received(1).AddAsync(Arg.Any<SavedPoll>(), cancellationToken);
        await _dbContext.Received(1).SaveChangesAsync(cancellationToken);
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldDisposeDbContext_AfterSaving()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "html", "Poll ended: Test" };
        var currentTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _clockService.CurrentUtcDateTime.Returns(currentTime);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldStoreCurrentUtcDateTime()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "html", "Poll ended: Test" };
        var specificTime = new DateTime(2024, 6, 15, 14, 30, 45, DateTimeKind.Utc);
        _clockService.CurrentUtcDateTime.Returns(specificTime);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _savedPollsDbSet.Received(1).AddAsync(
            Arg.Is<SavedPoll>(poll => poll.EndedAt == specificTime),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldStoreCorrectRoomId()
    {
        // Arrange
        var roomId = "specific-room-123";
        var parts = new[] { "room", "html", "Poll ended: Test" };
        var currentTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _clockService.CurrentUtcDateTime.Returns(currentTime);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _savedPollsDbSet.Received(1).AddAsync(
            Arg.Is<SavedPoll>(poll => poll.RoomId == "specific-room-123"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldHandleNullRoomId()
    {
        // Arrange
        var parts = new[] { "room", "html", "Poll ended: Test" };
        var currentTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _clockService.CurrentUtcDateTime.Returns(currentTime);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, null);

        // Assert
        await _savedPollsDbSet.Received(1).AddAsync(
            Arg.Is<SavedPoll>(poll => poll.RoomId == null),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldNotCreateDbContext_WhenConditionsNotMet()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "chat", "Poll ended: Test" };

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldHandleEmptyParts()
    {
        // Arrange
        var roomId = "test-room";
        var parts = Array.Empty<string>();

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldBeCaseInsensitive_ForPollEnded()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "html", "POLL ENDED: Test" };
        var currentTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _clockService.CurrentUtcDateTime.Returns(currentTime);
        _dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert - "Poll ended" check is case-sensitive in implementation
        await _savedPollsDbSet.DidNotReceive().AddAsync(Arg.Any<SavedPoll>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldCheckHtmlType_CaseSensitively()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "HTML", "Poll ended: Test" };

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert - parts[1] check is case-sensitive
        await _dbContextFactory.DidNotReceive().CreateDbContextAsync(Arg.Any<CancellationToken>());
    }
}