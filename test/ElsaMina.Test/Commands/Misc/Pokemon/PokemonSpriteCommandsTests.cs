using ElsaMina.Core.Contexts;
using ElsaMina.Commands.Misc.Pokemon;
using NSubstitute;

namespace ElsaMina.Test.Commands.Misc.Pokemon;

[TestFixture]
public class AfdSpriteCommandTest
{
    private IContext _context;
    private AfdSpriteCommand _command;
    private const string TEST_POKEMON = "pikachu";

    [SetUp]
    public void Setup()
    {
        _context = Substitute.For<IContext>();
        _context.Target.Returns(TEST_POKEMON);
        _command = new AfdSpriteCommand();
    }

    [Test]
    public async Task Test_RunAsync_ShouldReturnFrontSprite_WhenPokemonExists()
    {
        // Arrange
        _context.Command.Returns("afd");
        var expectedUrl = $"https://play.pokemonshowdown.com/sprites/afd/{TEST_POKEMON}.png";
        var expectedHtml = $"""<img src="{expectedUrl}" width="80" height="80" alt="{TEST_POKEMON} front sprite">""";

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyHtml(expectedHtml, null, true);
        _context.DidNotReceive().ReplyLocalizedMessage(Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReturnBackSprite_WhenPokemonExists()
    {
        // Arrange
        _context.Command.Returns("afd-back");
        var expectedUrl = $"https://play.pokemonshowdown.com/sprites/afd-back/{TEST_POKEMON}.png";
        var expectedHtml = $"""<img src="{expectedUrl}" width="80" height="80" alt="{TEST_POKEMON} back sprite">""";

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyHtml(expectedHtml, null, true);
        _context.DidNotReceive().ReplyLocalizedMessage(Arg.Any<string>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldReturnRequiredMessage_WhenNoTargetProvided()
    {
        // Arrange
        _context.Command.Returns("afd");
        _context.Target.Returns(string.Empty);

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("pokemon_name_required");
        _context.DidNotReceive().ReplyHtml(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
    }
} 