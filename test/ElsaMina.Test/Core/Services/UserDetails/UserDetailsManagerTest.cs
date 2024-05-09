using ElsaMina.Core;
using ElsaMina.Core.Services.System;
using ElsaMina.Core.Services.UserDetails;
using NSubstitute;

namespace ElsaMina.Test.Core.Services.UserDetails;

public class UserDetailsManagerTest
{
    private IClient _client;
    private ISystemService _systemService;

    private UserDetailsManager _userDetailsManager;

    [SetUp]
    public void SetUp()
    {
        _client = Substitute.For<IClient>();
        _systemService = Substitute.For<ISystemService>();

        _userDetailsManager = new UserDetailsManager(_client, _systemService);
    }

    [Test]
    public async Task Test_GetUserDetails_ShouldReturnTaskResolved_WhenUserDetailsAreReceived()
    {
        // Arrange
        var tcs = new TaskCompletionSource();
        _systemService.SleepAsync(Arg.Any<int>()).Returns(tcs.Task);
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
        // Arrange
        // TODO : revoir ce test
        _systemService.SleepAsync(Arg.Any<int>()).Returns(Task.Delay(TimeSpan.FromSeconds(1)));
        
        // Act
        var result = await _userDetailsManager.GetUserDetails("speks");
        
        // Assert
        Assert.That(result, Is.Null);
    }
}