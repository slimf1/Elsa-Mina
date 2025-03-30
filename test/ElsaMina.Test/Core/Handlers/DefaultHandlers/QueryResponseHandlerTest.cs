using ElsaMina.Core.Handlers.DefaultHandlers;
using ElsaMina.Core.Services.UserDetails;
using NSubstitute;

namespace ElsaMina.Test.Core.Handlers.DefaultHandlers;

public class QueryResponseHandlerTest
{
    private IUserDetailsManager _userDetailsManager;
    private QueryResponseHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _userDetailsManager = Substitute.For<IUserDetailsManager>();
        _handler = new QueryResponseHandler(_userDetailsManager);
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldCallHandleReceivedUserDetails_WhenCommandIsQueryResponseUserDetails()
    {
        // Arrange
        string[] parts = ["", "queryresponse", "userdetails", "userDetailsData"];

        // Act
        await _handler.HandleReceivedMessageAsync(parts);

        // Assert
        _userDetailsManager.Received(1).HandleReceivedUserDetails("userDetailsData");
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNotCallHandleReceivedUserDetails_WhenPartsLengthIsLessThanFour()
    {
        // Arrange
        string[] parts = ["", "queryresponse", "userdetails"];

        // Act
        await _handler.HandleReceivedMessageAsync(parts);

        // Assert
        _userDetailsManager.DidNotReceive().HandleReceivedUserDetails(Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNotCallHandleReceivedUserDetails_WhenSecondPartIsNotQueryResponse()
    {
        // Arrange
        string[] parts = ["", "notqueryresponse", "userdetails", "userDetailsData"];

        // Act
        await _handler.HandleReceivedMessageAsync(parts);

        // Assert
        _userDetailsManager.DidNotReceive().HandleReceivedUserDetails(Arg.Any<string>());
    }

    [Test]
    public async Task Test_HandleReceivedMessage_ShouldNotCallHandleReceivedUserDetails_WhenThirdPartIsNotUserDetails()
    {
        // Arrange
        string[] parts = ["", "queryresponse", "notuserdetails", "userDetailsData"];

        // Act
        await _handler.HandleReceivedMessageAsync(parts);

        // Assert
        _userDetailsManager.DidNotReceive().HandleReceivedUserDetails(Arg.Any<string>());
    }
}