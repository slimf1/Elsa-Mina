using ElsaMina.Commands.Showdown.Ranking;
using ElsaMina.Core.Services.Http;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Showdown.Ranking;

[TestFixture]
public class ShowdownRanksProviderTest
{
    private const string RANK_RESOURCE_URL =
        "https://play.pokemonshowdown.com/~~showdown/action.php?act=ladderget&user={0}";

    private IHttpService _httpService;
    private ShowdownRanksProvider _provider;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _provider = new ShowdownRanksProvider(_httpService);
    }

    [Test]
    public async Task Test_GetRankingDataAsync_ShouldCallGetAsyncWithFormattedUrl()
    {
        var mockResponse = Substitute.For<IHttpResponse<IEnumerable<RankingDataDto>>>();
        mockResponse.Data.Returns([]);
        _httpService
            .GetAsync<IEnumerable<RankingDataDto>>(Arg.Any<string>(), Arg.Any<IDictionary<string, string>>(),
                Arg.Any<IDictionary<string, string>>(), Arg.Any<bool>(), Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(mockResponse);

        await _provider.GetRankingDataAsync("testuser");

        await _httpService.Received(1).GetAsync<IEnumerable<RankingDataDto>>(
            string.Format(RANK_RESOURCE_URL, "testuser"),
            Arg.Any<IDictionary<string, string>>(),
            Arg.Any<IDictionary<string, string>>(),
            true,
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_GetRankingDataAsync_ShouldReturnDataFromResponse()
    {
        var expectedRankings = new List<RankingDataDto>
        {
            new() { FormatId = "gen9ou", Elo = 1500, Wins = 10, Losses = 5 },
            new() { FormatId = "gen8ou", Elo = 1400, Wins = 8, Losses = 3 }
        };
        var mockResponse = Substitute.For<IHttpResponse<IEnumerable<RankingDataDto>>>();
        mockResponse.Data.Returns(expectedRankings);
        _httpService
            .GetAsync<IEnumerable<RankingDataDto>>(Arg.Any<string>(), Arg.Any<IDictionary<string, string>>(),
                Arg.Any<IDictionary<string, string>>(), Arg.Any<bool>(), Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(mockResponse);

        var result = await _provider.GetRankingDataAsync("testuser");

        Assert.That(result, Is.EquivalentTo(expectedRankings));
    }

    [Test]
    public async Task Test_GetRankingDataAsync_ShouldPassCancellationToken()
    {
        var mockResponse = Substitute.For<IHttpResponse<IEnumerable<RankingDataDto>>>();
        mockResponse.Data.Returns([]);
        _httpService
            .GetAsync<IEnumerable<RankingDataDto>>(Arg.Any<string>(), Arg.Any<IDictionary<string, string>>(),
                Arg.Any<IDictionary<string, string>>(), Arg.Any<bool>(), Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(mockResponse);
        using var cts = new CancellationTokenSource();

        await _provider.GetRankingDataAsync("testuser", cts.Token);

        await _httpService.Received(1).GetAsync<IEnumerable<RankingDataDto>>(
            Arg.Any<string>(),
            Arg.Any<IDictionary<string, string>>(),
            Arg.Any<IDictionary<string, string>>(),
            Arg.Any<bool>(),
            Arg.Any<bool>(),
            cts.Token);
    }
}
