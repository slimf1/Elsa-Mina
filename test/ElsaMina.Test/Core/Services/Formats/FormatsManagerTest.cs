using ElsaMina.Core.Services.Formats;

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
        Assert.That(_formatsManager.Formats, Is.Not.Empty);
        Assert.That(_formatsManager.Formats.Count(), Is.EqualTo(2));
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
        Assert.That(tier, Is.EqualTo("[Gen 9] Random Battle"));
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
        Assert.That(tier, Is.EqualTo("gen7littlecup"));
    }
}