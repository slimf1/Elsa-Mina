using ElsaMina.Core.Services.Dex;

namespace ElsaMina.UnitTests.Core.Services.Dex;

public class DexManagerTest
{
    private DexManager _dexManager;

    [SetUp]
    public void SetUp()
    {
        _dexManager = new DexManager();
    }

    [Test]
    public void Test_Pokedex_ShouldBeEmpty_Initially()
    {
        Assert.That(_dexManager.Pokedex, Is.Empty);
    }

    [Test]
    public void Test_Moves_ShouldBeEmpty_Initially()
    {
        Assert.That(_dexManager.Moves, Is.Empty);
    }

    [Test]
    public async Task Test_LoadDexAsync_ShouldPopulateCollections()
    {
        // Act
        await _dexManager.LoadDexAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_dexManager.Pokedex, Is.Not.Empty);
            Assert.That(_dexManager.Moves, Is.Not.Empty);
        });
    }

    [Test]
    public async Task Test_LoadDexAsync_ShouldKeyPokedexByLowercaseName()
    {
        // Act
        await _dexManager.LoadDexAsync();

        // Assert
        Assert.That(_dexManager.Pokedex.ContainsKey("bulbasaur"), Is.True);
    }

    [Test]
    public async Task Test_LoadDexAsync_ShouldNotThrow()
    {
        Assert.DoesNotThrowAsync(async () => await _dexManager.LoadDexAsync());
    }
}
