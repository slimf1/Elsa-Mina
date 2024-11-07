using ElsaMina.Core.Utils;

namespace ElsaMina.Test.Core.Utils;

public class ParsingTest
{
    [Test]
    public void Test_ParseMessage_ShouldReturnNulls_WhenMessageDoesNotStartWithTrigger()
    {
        // Arrange
        var message = "Some message";
        var trigger = "!";

        // Act
        var (target, command) = Parsing.ParseMessage(message, trigger);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(target, Is.Null);
            Assert.That(command, Is.Null);
        });
    }

    [Test]
    public void Test_ParseMessage_ShouldReturnTargetAndCommand_WhenMessageStartsWithTrigger()
    {
        // Arrange
        var message = "!command argument";
        var trigger = "!";

        // Act
        var result = Parsing.ParseMessage(message, trigger);

        // Assert
        Assert.That(result, Is.EqualTo(("argument", "command")));
    }

    [Test]
    public void Test_ParseMessage_ShouldReturnEmptyTarget_WhenNoSpaceAfterCommand()
    {
        // Arrange
        var message = "!command";
        var trigger = "!";

        // Act
        var result = Parsing.ParseMessage(message, trigger);

        // Assert
        Assert.That(result, Is.EqualTo(("", "command")));
    }

    [Test]
    public void Test_ParseMessage_ShouldReturnNulls_WhenCommandIsEmptyAfterTrigger()
    {
        // Arrange
        var message = "! ";
        var trigger = "!";

        // Act
        var (target, command) = Parsing.ParseMessage(message, trigger);
        Assert.Multiple(() =>
        {

            // Assert
            Assert.That(target, Is.Null);
            Assert.That(command, Is.Null);
        });
    }
}