﻿using ElsaMina.Commands.Badges;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.RoomUserData;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.UnitTests.Commands.Badges;

public class TakeBadgeTest
{
    private IContext _context;
    private IRoomUserDataService _roomUserDataService;
    private TakeBadge _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _roomUserDataService = Substitute.For<IRoomUserDataService>();
        _command = new TakeBadge(_roomUserDataService);
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithHelpMessage_WhenInvalidArguments()
    {
        // Arrange
        _context.Target.Returns("invalid");
        _context.RoomId.Returns("roomId");

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("takebadge_help_message");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithBadgeDoesntExistMessage_WhenBadgeNotFound()
    {
        // Arrange
        _context.Target.Returns("userId,nonExistingBadge");
        _context.RoomId.Returns("roomId");
        _roomUserDataService
            .TakeBadgeFromUser("roomId", "userid", "nonexistingbadge")
            .Throws(new ArgumentException());

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("takebadge_badge_doesnt_exist", "userid", "nonexistingbadge");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithSuccessMessage_WhenBadgeSuccessfullyTaken()
    {
        // Arrange
        _context.Target.Returns("userId,existingBadge");
        _context.RoomId.Returns("roomId");

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _roomUserDataService.Received().TakeBadgeFromUser("roomId", "userid", "existingbadge");
        _context.Received().ReplyLocalizedMessage("takebadge_success", "userid", "existingbadge");
    }

    [Test]
    public async Task Test_RunAsync_ShouldReplyWithErrorMessage_WhenExceptionThrown()
    {
        // Arrange
        _context.Target.Returns("userId,existingBadge");
        _context.RoomId.Returns("roomId");
        _roomUserDataService
            .TakeBadgeFromUser("roomId", "userid", "existingbadge")
            .Throws(new Exception("Some error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received().ReplyLocalizedMessage("takebadge_failure", "Some error");
    }
}