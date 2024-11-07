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
        var mockResponse = new HttpResponse<CoinDeskResponseDto>
        {
            Data = new CoinDeskResponseDto
            {
                Bpi = new Dictionary<string, BpiDto>
                {
                    ["EUR"] = new() { Rate = 40000.50 },
                    ["USD"] = new() { Rate = 42000.75 }
                }
            }
        };

        _httpService.Get<CoinDeskResponseDto>(Arg.Any<string>())
            .Returns(mockResponse);

        // Act
        await _bitcoinCommand.Run(_context);

        // Assert
        _context.Received().Reply("1 bitcoin = 40000.50€ = 42000.75$", rankAware: true);
    }

    [Test]
    public async Task Test_Run_ShouldLogError_WhenApiCallFails()
    {
        // Arrange
        _httpService.Get<CoinDeskResponseDto>(Arg.Any<string>())
            .Throws(new Exception("API error"));

        // Act
        await _bitcoinCommand.Run(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("bitcoin_error");
    }
}
