using ElsaMina.Core.Utils;
using Newtonsoft.Json;

namespace ElsaMina.UnitTests.Core.Utils;

public class ShowdownTeamsTest
{
    [Test]
    public void Test_DeserializeTeamExport_ShouldReturnEmptyTeam_WhenExportIsEmpty()
    {
        // Arrange
        var export = string.Empty;

        // Act
        var result = ShowdownTeams.DeserializeTeamExport(export);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Test_DeserializeTeamExport_ShouldParseSinglePokemon_WhenValidSinglePokemonExportIsGiven()
    {
        // Arrange
        var export = @"Pikachu @ Light Ball
Ability: Static
EVs: 252 Atk / 4 SpD / 252 Spe
Jolly Nature
- Volt Tackle
- Iron Tail";

        // Act
        var result = ShowdownTeams.DeserializeTeamExport(export).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        var pokemonSet = result.First();
        Assert.Multiple(() =>
        {
            Assert.That(pokemonSet.Species, Is.EqualTo("Pikachu"));
            Assert.That(pokemonSet.Item, Is.EqualTo("Light Ball"));
            Assert.That(pokemonSet.Ability, Is.EqualTo("Static"));
            Assert.That(pokemonSet.Nature, Is.EqualTo("Jolly"));
            Assert.That(pokemonSet.EffortValues["atk"], Is.EqualTo(252));
            Assert.That(pokemonSet.EffortValues["spe"], Is.EqualTo(252));
            Assert.That(pokemonSet.Moves, Is.EquivalentTo(new List<string> { "Volt Tackle", "Iron Tail" }));
        });
    }

    [Test]
    public void Test_GetSetExport_ShouldGenerateCorrectFormat_WhenPokemonSetIsGiven()
    {
        // Arrange
        var pokemonSet = new PokemonSet
        {
            Species = "Pikachu",
            Item = "Light Ball",
            Ability = "Static",
            Nature = "Jolly",
            EffortValues = new Dictionary<string, int> { { "atk", 252 }, { "spe", 252 } },
            Moves = new List<string> { "Volt Tackle", "Iron Tail" }
        };

        // Act
        var result = ShowdownTeams.GetSetExport(pokemonSet);

        // Assert
        var expectedExport = @"Pikachu @ Light Ball
Ability: Static 
EVs: 252 Atk / 252 Spe
Jolly Nature 
- Volt Tackle
- Iron Tail";
        Assert.That(result.Trim(), Is.EqualTo(expectedExport));
    }

    [Test]
    public void Test_TeamExportToJson_ShouldReturnJsonRepresentation_WhenExportIsGiven()
    {
        // Arrange
        var export = @"Pikachu @ Light Ball
Ability: Static
EVs: 252 Atk / 4 SpD / 252 Spe
Jolly Nature
- Volt Tackle
- Iron Tail";

        var expectedJson = JsonConvert.SerializeObject(ShowdownTeams.DeserializeTeamExport(export));

        // Act
        var result = ShowdownTeams.TeamExportToJson(export);

        // Assert
        Assert.That(result, Is.EqualTo(expectedJson));
    }


    [Test]
    public void Test_DeserializeTeamExport_ShouldParseMultiplePokemon_WhenMultiplePokemonExportIsGiven()
    {
        // Arrange
        var export = @"Pikachu @ Light Ball
Ability: Static
EVs: 252 Atk / 4 SpD / 252 Spe
Jolly Nature
- Volt Tackle
- Iron Tail

Charizard @ Charizardite X
Ability: Blaze
EVs: 4 HP / 252 Atk / 252 Spe
Adamant Nature
- Flare Blitz
- Dragon Claw";

        // Act
        var result = ShowdownTeams.DeserializeTeamExport(export).ToList();

        // Assert
        var expectedMoves = new[] { "Flare Blitz", "Dragon Claw" };
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Species, Is.EqualTo("Pikachu"));
            Assert.That(result[1].Species, Is.EqualTo("Charizard"));
            Assert.That(result[1].Item, Is.EqualTo("Charizardite X"));
            Assert.That(result[1].Ability, Is.EqualTo("Blaze"));
            Assert.That(result[1].Nature, Is.EqualTo("Adamant"));
            Assert.That(result[1].Moves, Is.EquivalentTo(expectedMoves));
        });
    }
}