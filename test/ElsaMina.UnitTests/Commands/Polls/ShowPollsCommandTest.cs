using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System.Globalization;
using ElsaMina.Commands.Polls;

namespace ElsaMina.UnitTests.Commands.Polls;

public class ShowPollsCommandTest
{
    private IRoomsManager _roomsManager;
    private IContext _context;
    private IBotDbContextFactory _dbFactory;
    private BotDbContext _dbContext;
    private DbSet<SavedPoll> _pollsSet;
    private ShowPollsCommand _command;

    [SetUp]
    public void SetUp()
    {
        _roomsManager = Substitute.For<IRoomsManager>();
        _context = Substitute.For<IContext>();
        _context.Culture.Returns(new CultureInfo("en-US"));

        _dbFactory = Substitute.For<IBotDbContextFactory>();
        _dbContext = Substitute.For<BotDbContext>();
        _dbFactory.CreateDbContextAsync(Arg.Any<CancellationToken>()).Returns(_dbContext);

        _command = new ShowPollsCommand(_roomsManager, _dbFactory);
    }

    private void MockPolls(IEnumerable<SavedPoll> polls)
    {
        var queryable = polls.AsQueryable();

        _pollsSet = Substitute.For<DbSet<SavedPoll>, IQueryable<SavedPoll>>();

        ((IQueryable<SavedPoll>)_pollsSet).Provider.Returns(queryable.Provider);
        ((IQueryable<SavedPoll>)_pollsSet).Expression.Returns(queryable.Expression);
        ((IQueryable<SavedPoll>)_pollsSet).ElementType.Returns(queryable.ElementType);
        ((IQueryable<SavedPoll>)_pollsSet).GetEnumerator().Returns(queryable.GetEnumerator());

        _dbContext.SavedPolls.Returns(_pollsSet);
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
            new() { Id = 1, Content = "Poll 1", RoomId = roomId, EndedAt = new DateTimeOffset(2025,2,4,8,20,0,TimeSpan.Zero) },
            new() { Id = 2, Content = "Poll 2", RoomId = roomId, EndedAt = new DateTimeOffset(2025,2,4,8,30,0,TimeSpan.Zero) }
        };

        MockPolls(polls);

        _context.GetString("show_polls_history_header", Arg.Any<object[]>())
            .Returns(call => $"Poll history for {call.Arg<object[]>()[0]}:<br>");

        _context.GetString("show_polls_history_entry", Arg.Any<object[]>())
            .Returns(call =>
                $"ID: {call.Arg<object[]>()[0]}, Ended At: {call.Arg<object[]>()[1]}, Content: {call.Arg<object[]>()[2]}");

        _context.GetString("show_polls_history_sent").Returns("Poll history sent.");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("show_polls_history_sent");

        _context.Received(1).ReplyHtmlPage(
            "polls-history",
            Arg.Is<string>(msg =>
                msg.Contains("Poll history for test-room") &&
                msg.Contains("ID: 1") &&
                msg.Contains("Poll 1") &&
                msg.Contains("ID: 2") &&
                msg.Contains("Poll 2")
            ));
    }

    [Test]
    public async Task Test_Run_ShouldShowError_WhenRoomDoesNotExist()
    {
        const string roomId = "missing-room";
        _context.Target.Returns(roomId);
        _roomsManager.HasRoom(roomId).Returns(false);

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("show_polls_room_not_exist", roomId);
        _context.DidNotReceive().ReplyHtmlPage(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task Test_Run_ShouldShowError_WhenNoPollsExist()
    {
        const string roomId = "test-room";
        _context.Target.Returns(string.Empty);
        _context.RoomId.Returns(roomId);

        _roomsManager.HasRoom(roomId).Returns(true);

        MockPolls(new List<SavedPoll>()); // empty

        await _command.RunAsync(_context);

        _context.Received(1).ReplyLocalizedMessage("show_polls_no_polls", roomId);
        _context.DidNotReceive().ReplyHtmlPage(Arg.Any<string>(), Arg.Any<string>());
    }
}
