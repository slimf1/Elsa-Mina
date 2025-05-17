using ElsaMina.Core.Commands;
using ElsaMina.Core.Contexts;
using NSubstitute;

namespace ElsaMina.Test.Core.Commands;

public class CommandTest
{
    private Action _action;
    private TestCommand _testCommand;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _action = Substitute.For<Action>();
        _testCommand = new TestCommand(_action);
        _context = Substitute.For<IContext>();
    }

    [Test]
    public void Test_Constructor_ShouldInitializePropertiesFromAttribute()
    {
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_testCommand.Name, Is.EqualTo("test-command"));
            Assert.That(_testCommand.Aliases, Is.EquivalentTo(new List<string> { "alias1", "alias2" }));
        });
    }

    [Test]
    public void Test_ReplyLocalizedHelpMessage_ShouldReplyLocalizedHelpMessage_WhenCalledWithHelpMessageKey()
    {
        // Arrange
        _testCommand.CommandHelpMessageKey = "Help.Key";
        _context.GetString("Help.Key").Returns("Localized Help Message");

        // Act
        _testCommand.ReplyLocalizedHelpMessage(_context);

        // Assert
        _context.Received(1).Reply("Localized Help Message");
    }

    [NamedCommand("test-command", "alias1", "alias2")]
    private class TestCommand : Command
    {
        private readonly Action _action;

        public TestCommand(Action action)
        {
            _action = action;
        }

        public string CommandHelpMessageKey { get; set; }

        public override string HelpMessageKey => CommandHelpMessageKey;

        public override Task RunAsync(IContext context, CancellationToken cancellationToken = default)
        {
            _action.Invoke();
            return Task.CompletedTask;
        }
    }
}