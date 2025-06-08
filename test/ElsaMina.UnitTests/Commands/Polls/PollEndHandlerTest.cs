using ElsaMina.Core.Services.Clock;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using ElsaMina.Commands.Polls;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Polls;

public class PollEndHandlerTest
{
    private ISavedPollRepository _savedPollRepository;
    private IClockService _clockService;
    private PollEndHandler _pollEndHandler;

    [SetUp]
    public void SetUp()
    {
        _savedPollRepository = Substitute.For<ISavedPollRepository>();
        _clockService = Substitute.For<IClockService>();
        _pollEndHandler = new PollEndHandler(_savedPollRepository, _clockService);
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldSavePoll_WhenPollEndedInEnglish()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "html", "Poll ended: What is your favorite color?" };
        var currentTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _clockService.CurrentUtcDateTime.Returns(currentTime);

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _savedPollRepository.Received(1).AddAsync(Arg.Is<SavedPoll>(poll =>
            poll.RoomId == roomId &&
            poll.Content == "Poll ended: What is your favorite color?" &&
            poll.EndedAt == currentTime));
    }

    [Test]
    public async Task Test_HandleReceivedMessageAsync_ShouldSavePoll_WhenPollEndedInFrench()
    {
        // Arrange
        var roomId = "test-room";
        var parts = new[] { "room", "html", "Sondage terminé: Quelle est votre couleur préférée?" };
        var currentTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _clockService.CurrentUtcDateTime.Returns(currentTime);

        // Act
        await _pollEndHandler.HandleReceivedMessageAsync(parts, roomId);

        // Assert
        await _savedPollRepository.Received(1).AddAsync(Arg.Is<SavedPoll>(poll =>
            poll.RoomId == roomId &&
            poll.Content == "Sondage terminé: Quelle est votre couleur préférée?" &&
            poll.EndedAt == currentTime));
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
        await _savedPollRepository.DidNotReceive().AddAsync(Arg.Any<SavedPoll>());
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
        await _savedPollRepository.DidNotReceive().AddAsync(Arg.Any<SavedPoll>());
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
        await _savedPollRepository.DidNotReceive().AddAsync(Arg.Any<SavedPoll>());
    }
} 