using ElsaMina.Commands.Teams.TeamProviders;
using ElsaMina.Core.Services.DependencyInjection;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ElsaMina.Test.Commands.Teams.TeamProviders;

public class TeamLinkMatchFactoryTests
{
    private IDependencyContainerService _dependencyContainerService;
    private ITeamProvider _teamProvider1;
    private ITeamProvider _teamProvider2;
    private TeamLinkMatchFactory _factory;

    [SetUp]
    public void SetUp()
    {
        _dependencyContainerService = Substitute.For<IDependencyContainerService>();
        _teamProvider1 = Substitute.For<ITeamProvider>();
        _teamProvider2 = Substitute.For<ITeamProvider>();

        _factory = new TeamLinkMatchFactory(_dependencyContainerService);
    }

    [Test]
    public async Task Test_FindTeamLinkMatch_ShouldReturnTeamLinkMatch_WhenATeamProviderMatchesLink()
    {
        // Arrange
        const string message = "This is a message with a valid team link";
        const string expectedLink = "https://validteamlink.com";
        _teamProvider1.GetMatchFromLink(message).ReturnsNull();
        _teamProvider2.GetMatchFromLink(message).Returns(expectedLink);

        var teamProviders = new List<ITeamProvider> { _teamProvider1, _teamProvider2 };
        _dependencyContainerService.Resolve<IEnumerable<ITeamProvider>>().Returns(teamProviders);

        // Act
        var result = _factory.FindTeamLinkMatch(message);

        // Assert
        await result.GetTeamExport();
        Assert.That(result, Is.Not.Null);
        await _teamProvider2.Received(1).GetTeamExport(expectedLink);
        await _teamProvider1.DidNotReceive().GetTeamExport(Arg.Any<string>());
    }

    [Test]
    public void Test_FindTeamLinkMatch_ShouldReturnNull_WhenNoTeamProviderMatchesLink()
    {
        // Arrange
        const string message = "This message has no valid team link";
        _teamProvider1.GetMatchFromLink(message).Returns((string)null);
        _teamProvider2.GetMatchFromLink(message).Returns((string)null);

        var teamProviders = new List<ITeamProvider> { _teamProvider1, _teamProvider2 };
        _dependencyContainerService.Resolve<IEnumerable<ITeamProvider>>().Returns(teamProviders);

        // Act
        var result = _factory.FindTeamLinkMatch(message);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void Test_FindTeamLinkMatch_ShouldLazyLoadProviders_WhenCalled()
    {
        // Arrange
        var message = "Message with no link";
        var teamProviders = new List<ITeamProvider> { _teamProvider1, _teamProvider2 };
        _dependencyContainerService.Resolve<IEnumerable<ITeamProvider>>().Returns(teamProviders);
        _factory.FindTeamLinkMatch(message);

        // Act
        _factory.FindTeamLinkMatch(message); // Call again to test lazy loading

        // Assert
        _dependencyContainerService.Received(1).Resolve<IEnumerable<ITeamProvider>>();
    }
}
