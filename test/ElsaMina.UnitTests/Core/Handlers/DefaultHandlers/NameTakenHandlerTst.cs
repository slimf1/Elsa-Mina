using ElsaMina.Core.Handlers.DefaultHandlers;
using ElsaMina.Core.Services.System;
using NSubstitute;

namespace ElsaMina.UnitTests.Core.Handlers.DefaultHandlers;

public class NameTakenHandlerTest
{
    private ISystemService _systemServiceMock;
    private NameTakenHandler _nameTakenHandler;

    [SetUp]
    public void SetUp()
    {
        _systemServiceMock = Substitute.For<ISystemService>();
        _nameTakenHandler = new NameTakenHandler(_systemServiceMock);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldCallKill_WhenCommandIsNameTaken()
    {
        // Arrange
        string[] parts = ["", "nametaken"];

        // Act
        await _nameTakenHandler.HandleReceivedMessageAsync(parts);

        // Assert
        _systemServiceMock.Received(1).Kill();
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNotCallKill_WhenPartsLengthIsLessThanTwo()
    {
        // Arrange
        string[] parts = [""];

        // Act
        await _nameTakenHandler.HandleReceivedMessageAsync(parts);

        // Assert
        _systemServiceMock.DidNotReceive().Kill();
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNotCallKill_WhenSecondPartIsNotNameTaken()
    {
        // Arrange
        string[] parts = ["", "notnametaken"];

        // Act
        await _nameTakenHandler.HandleReceivedMessageAsync(parts);

        // Assert
        _systemServiceMock.DidNotReceive().Kill();
    }
}