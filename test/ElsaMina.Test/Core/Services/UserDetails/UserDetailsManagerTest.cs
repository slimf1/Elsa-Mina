using ElsaMina.Core.Client;
using ElsaMina.Core.Services.UserDetails;
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
    public async Task Test_GetUserDetails_ShouldReturnTaskResolved_WhenUserDetailsAreReceived()
    {
        // Arrange
        var task = _userDetailsManager.GetUserDetails("panur");
        _userDetailsManager.HandleReceivedUserDetails("""{"id":"panur","userid":"panur","name":"Panur","avatar":"sightseerf","group":"+","autoconfirmed":true}""");
        
        // Act
        var result = await task;
        
        // Assert
        Assert.That(result.Name, Is.EqualTo("Panur"));
        Assert.That(result.Avatar, Is.EqualTo("sightseerf"));
        Assert.That(result.Group, Is.EqualTo("+"));
        Assert.That(result.AutoConfirmed, Is.True);
    }
    
    [Test]
    public async Task Test_GetUserDetails_ShouldReturnNull_WhenUserDetailsAreNotReceived()
    {
        // Act
        var result = await _userDetailsManager.GetUserDetails("speks");
        
        // Assert
        Assert.That(result, Is.Null);
    }
}