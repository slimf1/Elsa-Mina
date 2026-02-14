using ElsaMina.Core.Utils;

namespace ElsaMina.UnitTests.Core.Utils;

public class TimeSpanStringExtensionsTest
{
    // --------------------
    // Valid single units
    // --------------------

    [Test]
    public void Test_ToTimeSpan_ShouldParseMinutes_WhenUsingPlural()
    {
        // Arrange
        var input = "3 minutes";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromMinutes(3)));
    }

    [Test]
    public void Test_ToTimeSpan_ShouldParseMinutes_WhenUsingAbbreviation()
    {
        // Arrange
        var input = "3 mins";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromMinutes(3)));
    }

    [Test]
    public void Test_ToTimeSpan_ShouldParseSeconds_WhenUsingShortForm()
    {
        // Arrange
        var input = "45s";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromSeconds(45)));
    }

    [Test]
    public void Test_ToTimeSpan_ShouldParseMilliseconds_WhenUsingMs()
    {
        // Arrange
        var input = "500ms";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(500)));
    }

    [Test]
    public void Test_ToTimeSpan_ShouldParseHours_WhenUsingSingleLetter()
    {
        // Arrange
        var input = "2h";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromHours(2)));
    }

    [Test]
    public void Test_ToTimeSpan_ShouldParseDays_WhenUsingShortForm()
    {
        // Arrange
        var input = "1d";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromDays(1)));
    }

    [Test]
    public void Test_ToTimeSpan_ShouldParseWeeks_WhenUsingShortForm()
    {
        // Arrange
        var input = "2w";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromDays(14)));
    }

    // --------------------
    // Multiple parts
    // --------------------

    [Test]
    public void Test_ToTimeSpan_ShouldParseMultipleUnits_WhenSpaceSeparated()
    {
        // Arrange
        var input = "1 hour 30 minutes";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromMinutes(90)));
    }

    [Test]
    public void Test_ToTimeSpan_ShouldParseMultipleUnits_WhenCompact()
    {
        // Arrange
        var input = "1h30m";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromMinutes(90)));
    }

    [Test]
    public void Test_ToTimeSpan_ShouldParseMultipleUnits_WhenOutOfOrder()
    {
        // Arrange
        var input = "30m 1h";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromMinutes(90)));
    }

    [Test]
    public void Test_ToTimeSpan_ShouldParseComplexCombination()
    {
        // Arrange
        var input = "2d 4h 15m 10s";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(
            result,
            Is.EqualTo(
                TimeSpan.FromDays(2)
                + TimeSpan.FromHours(4)
                + TimeSpan.FromMinutes(15)
                + TimeSpan.FromSeconds(10)
            )
        );
    }

    // --------------------
    // Decimals
    // --------------------

    [Test]
    public void Test_ToTimeSpan_ShouldParseDecimalHours()
    {
        // Arrange
        var input = "1.5h";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromMinutes(90)));
    }

    [Test]
    public void Test_ToTimeSpan_ShouldParseDecimalMinutes()
    {
        // Arrange
        var input = "0.5m";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromSeconds(30)));
    }

    // --------------------
    // Casing & whitespace
    // --------------------

    [Test]
    public void Test_ToTimeSpan_ShouldIgnoreCasing()
    {
        // Arrange
        var input = "2H 30M";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromMinutes(150)));
    }

    [Test]
    public void Test_ToTimeSpan_ShouldIgnoreExtraWhitespace()
    {
        // Arrange
        var input = "   1h    15m   ";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromMinutes(75)));
    }

    // --------------------
    // Invalid inputs
    // --------------------

    [Test]
    public void Test_ToTimeSpan_ShouldReturnNull_WhenInputIsNull()
    {
        // Arrange
        string input = null;

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Test_ToTimeSpan_ShouldReturnNull_WhenInputIsEmpty()
    {
        // Arrange
        var input = "";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Test_ToTimeSpan_ShouldReturnNull_WhenInputIsWhitespace()
    {
        // Arrange
        var input = "   ";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Test_ToTimeSpan_ShouldReturnNull_WhenNoUnitsPresent()
    {
        // Arrange
        var input = "123";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Test_ToTimeSpan_ShouldReturnNull_WhenUnknownUnitUsed()
    {
        // Arrange
        var input = "5 lightyears";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Test_ToTimeSpan_ShouldReturnNull_WhenGarbageTextPresent()
    {
        // Arrange
        var input = "1h abc";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Test_ToTimeSpan_ShouldReturnNull_WhenPartiallyValidInput()
    {
        // Arrange
        var input = "1h 30";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Test_ToTimeSpan_ShouldReturnNull_WhenInvalidNumber()
    {
        // Arrange
        var input = "1..5h";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.Null);
    }

    // --------------------
    // Edge cases
    // --------------------

    [Test]
    public void Test_ToTimeSpan_ShouldAllowZeroValues()
    {
        // Arrange
        var input = "0h 0m";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.Zero));
    }

    [Test]
    public void Test_ToTimeSpan_ShouldHandleRepeatedUnits()
    {
        // Arrange
        var input = "1h 30m 30m";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromMinutes(120)));
    }

    [Test]
    public void Test_ToTimeSpan_ShouldHandleMillisecondsAndSecondsTogether()
    {
        // Arrange
        var input = "1s 500ms";

        // Act
        var result = input.ToTimeSpan();

        // Assert
        Assert.That(result, Is.EqualTo(TimeSpan.FromMilliseconds(1500)));
    }
}