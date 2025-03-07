using ElsaMina.Commands.Misc.Bitcoin;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.Test.Commands.Misc.Bitcoin;

public class BitcoinCommandTests
{
    private IHttpService _httpService;
    private BitcoinCommand _bitcoinCommand;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _context = Substitute.For<IContext>();

        _bitcoinCommand = new BitcoinCommand(_httpService);
    }

    [Test]
    public void Test_IsAllowedInPrivateMessage_ShouldReturnTrue()
    {
        // Assert
        Assert.That(_bitcoinCommand.IsAllowedInPrivateMessage, Is.True);
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithBitcoinRates_WhenApiCallSucceeds()
    {
        // Arrange
        var mockResponse = new HttpResponse<IDictionary<string, IDictionary<string, int>>>
        {
            Data = new Dictionary<string, IDictionary<string, int>>
            {
                ["bitcoin"] = new Dictionary<string, int>
                {
                    ["eur"] = 40000,
                    ["usd"] = 42000
                }
            }
        };

        _httpService.GetAsync<IDictionary<string, IDictionary<string, int>>>(Arg.Any<string>())
            .Returns(mockResponse);

        // Act
        await _bitcoinCommand.Run(_context);

        // Assert
        _context.Received().Reply("1 bitcoin = 40000€ = 42000$", rankAware: true);
    }

    [Test]
    public async Task Test_Run_ShouldReplyWithError_WhenApiCallFails()
    {
        // Arrange
        _httpService.GetAsync<IDictionary<string, IDictionary<string, int>>>(Arg.Any<string>())
            .Throws(new Exception("API error"));

        // Act
        await _bitcoinCommand.Run(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("bitcoin_error");
    }
}
