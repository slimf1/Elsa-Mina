using ElsaMina.Core.Client;
using ElsaMina.Core.Services.UserDetails;
using FluentAssertions;
using NSubstitute;
using Serilog;

namespace ElsaMina.Test.Core.Services.UserDetails;

public class UserDetailsManagerTest
{
    private ILogger _logger;
    private IClient _client;

    private UserDetailsManager _userDetailsManager;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger>();
        _client = Substitute.For<IClient>();

        _userDetailsManager = new UserDetailsManager(_logger, _client);
    }

    [Test]
    public async Task Test_GetUserDetails_ShouldReturnTaskResolvedWhenUserDetailsAreReceived()
    {
        // Act
        var task = _userDetailsManager.GetUserDetails("panur");
        _userDetailsManager.HandleReceivedUserDetails("""{"id":"panur","userid":"panur","name":"Panur","avatar":"sightseerf","group":"+","autoconfirmed":true}""");
        var result = await task;
        
        // Assert
        result.Name.Should().Be("Panur");
        result.Avatar.Should().Be("sightseerf");
        result.Group.Should().Be("+");
        result.AutoConfirmed.Should().BeTrue();
    }
}