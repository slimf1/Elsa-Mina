using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using ElsaMina.Commands.Polls;
using NSubstitute;
using System.Globalization;

namespace ElsaMina.Test.Commands.Polls;

public class ShowPollsCommandTest
{
    private ISavedPollRepository _savedPollRepository;
    private IRoomsManager _roomsManager;
    private IContext _context;
    private ShowPollsCommand _showPollsCommand;

    [SetUp]
    public void SetUp()
    {
        _savedPollRepository = Substitute.For<ISavedPollRepository>();
        _roomsManager = Substitute.For<IRoomsManager>();
        _context = Substitute.For<IContext>();
        _context.Culture.Returns(new CultureInfo("en-US"));
        _showPollsCommand = new ShowPollsCommand(_savedPollRepository, _roomsManager);
    }

    [Test]
    public async Task Test_Run_ShouldShowPolls_WhenRoomExists()
    {
        // Arrange
        const string roomId = "test-room";
        _context.RoomId.Returns(roomId);
        _context.Target.Returns(string.Empty);
        _roomsManager.HasRoom(roomId).Returns(true);

        var polls = new List<SavedPoll>
        {
            new() { Id = 1, Content = "Poll 1", RoomId = roomId, EndedAt = new DateTimeOffset(2025, 2, 4, 8, 20, 0, TimeSpan.Zero) },
            new() { Id = 2, Content = "Poll 2", RoomId = roomId, EndedAt = new DateTimeOffset(2025, 2, 4, 8, 30, 0, TimeSpan.Zero) }
        };
        _savedPollRepository.GetPollsByRoomIdAsync(roomId, Arg.Any<CancellationToken>()).Returns(polls);

        // Mock GetString for all resource keys used in the test
        _context.GetString("show_polls_history_header", Arg.Any<object[]>()).Returns(x => string.Format("Poll history for {0}:<br>", x.Arg<object[]>()));
        _context.GetString("show_polls_history_entry", Arg.Any<object[]>()).Returns(x => string.Format("ID: {0}, Ended At: {1}, Content: {2}", x.Arg<object[]>()));
        _context.GetString("show_polls_history_sent").Returns("Poll history sent.");

        // Act
        await _showPollsCommand.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("show_polls_history_sent");
        _context.Received(1).ReplyHtmlPage("polls-history", Arg.Is<string>(message =>
            message.Contains("Poll history for test-room:") &&
            message.Contains("ID: 1") &&
            message.Contains("Content: Poll 1") &&
            message.Contains("ID: 2") &&
            message.Contains("Content: Poll 2")));
    }

    [Test]
    public async Task Test_Run_ShouldShowError_WhenRoomDoesNotExist()
    {
        // Arrange
        const string roomId = "non-existent-room";
        _context.Target.Returns(roomId);
        _roomsManager.HasRoom(roomId).Returns(false);

        // Act
        await _showPollsCommand.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("show_polls_room_not_exist", roomId);
        _context.DidNotReceive().ReplyHtmlPage(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_Run_ShouldShowError_WhenNoPollsExist()
    {
        // Arrange
        const string roomId = "test-room";
        _context.RoomId.Returns(roomId);
        _context.Target.Returns(string.Empty);
        _roomsManager.HasRoom(roomId).Returns(true);
        _savedPollRepository.GetPollsByRoomIdAsync(roomId, Arg.Any<CancellationToken>()).Returns(new List<SavedPoll>());

        // Act
        await _showPollsCommand.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("show_polls_no_polls", roomId);
        _context.DidNotReceive().ReplyHtmlPage(Arg.Any<string>(), Arg.Any<string>());
    }
} 