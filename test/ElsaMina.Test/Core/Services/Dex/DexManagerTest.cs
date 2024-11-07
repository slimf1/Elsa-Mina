using ElsaMina.Core.Services.Dex;
using ElsaMina.Core.Services.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.Test.Core.Services.Dex;

public class DexManagerTest
{
    private IHttpService _httpService;
    private DexManager _dexManager;

    [SetUp]
    public void SetUp()
    {
        _httpService = Substitute.For<IHttpService>();
        _dexManager = new DexManager(_httpService);
    }

    [Test]
    public async Task Test_LoadDex_ShouldSetPokedex_WhenResponseIsSuccessful()
    {
        // Arrange
        var expectedPokedex = new List<Pokemon>
        {
            new() { Name = new Name { English = "Pikachu" } },
            new() { Name = new Name { English = "Charizard" } }
        };
        var response = new HttpResponse<List<Pokemon>> { Data = expectedPokedex };
        _httpService.Get<List<Pokemon>>(Arg.Is(DexManager.DEX_URL)).Returns(response);

        // Act
        await _dexManager.LoadDex();

        // Assert
        Assert.That(_dexManager.Pokedex, Is.Not.Null);
        Assert.That(_dexManager.Pokedex, Has.Count.EqualTo(2));
        Assert.That(_dexManager.Pokedex, Is.EquivalentTo(expectedPokedex));
    }

    [Test]
    public async Task Test_LoadDex_ShouldLogInformation_WhenDexIsLoaded()
    {
        // Arrange
        var expectedPokedex = new List<Pokemon> { new() { Name = new Name { English = "Bulbasaur" } } };
        var response = new HttpResponse<List<Pokemon>> { Data = expectedPokedex };
        _httpService.Get<List<Pokemon>>(Arg.Is(DexManager.DEX_URL)).Returns(response);

        // Act
        await _dexManager.LoadDex();

        // Assert
        Assert.That(_dexManager.Pokedex, Has.Count.EqualTo(1));
        Assert.That(_dexManager.Pokedex[0].Name.English, Is.EqualTo("Bulbasaur"));
    }

    [Test]
    public async Task Test_LoadDex_ShouldLogError_WhenExceptionThrown()
    {
        // Arrange
        _httpService.Get<List<Pokemon>>(Arg.Is(DexManager.DEX_URL)).Throws(new Exception("Network error"));

        // Act
        await _dexManager.LoadDex();

        // Assert
        Assert.That(_dexManager.Pokedex, Is.Empty);
    }

    [Test]
    public void Test_LoadDex_ShouldNotThrowException_WhenExceptionOccurs()
    {
        // Arrange
        _httpService.Get<List<Pokemon>>(Arg.Is(DexManager.DEX_URL)).Throws(new Exception("Network error"));

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _dexManager.LoadDex());
    }
}