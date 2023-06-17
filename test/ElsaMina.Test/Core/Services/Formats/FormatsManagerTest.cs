using ElsaMina.Core.Services.Formats;
using FluentAssertions;

namespace ElsaMina.Test.Core.Services.Formats;

public class FormatsManagerTest
{
    private FormatsManager _formatsManager;

    [SetUp]
    public void SetUp()
    {
        _formatsManager = new FormatsManager();
    }

    [Test]
    public void Test_ParseFormatsFromReceivedLine_ShouldInsertFormats()
    {
        // Arrange
        const string message = "|formats|,1|S/V Singles|[Gen 9] Random Battle,f|[Gen 9] Unrated Random Battle,b";

        // Act
        _formatsManager.ParseFormatsFromReceivedLine(message);
        
        // Assert
        _formatsManager.Formats.Should().NotBeNullOrEmpty();
        _formatsManager.Formats.Count().Should().Be(2);
    }

    [Test]
    public void Test_GetFormattedTier_ShouldGetFormattedTier_WhenTierExists()
    {
        // Arrange
        const string message = "|formats|,1|S/V Singles|[Gen 9] Random Battle,f|[Gen 9] Unrated Random Battle,b";
        _formatsManager.ParseFormatsFromReceivedLine(message);

        // Act
        var tier = _formatsManager.GetFormattedTier("gen9randombattle");
        
        // Assert
        tier.Should().Be("[Gen 9] Random Battle");
    }
    
    [Test]
    public void Test_GetFormattedTier_ShouldGetInputTier_WhenTierDoesntExists()
    {
        // Arrange
        const string message = "|formats|,1|S/V Singles|[Gen 9] Random Battle,f|[Gen 9] Unrated Random Battle,b";
        _formatsManager.ParseFormatsFromReceivedLine(message);

        // Act
        var tier = _formatsManager.GetFormattedTier("gen7littlecup");
        
        // Assert
        tier.Should().Be("gen7littlecup");
    }
}