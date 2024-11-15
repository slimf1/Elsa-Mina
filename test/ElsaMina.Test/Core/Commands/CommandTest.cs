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
    public async Task Test_Call_ShouldNotRun_WhenIsPrivateMessageOnlyAndContextNotPrivateMessage()
    {
        // Arrange
        _context.IsPrivateMessage.Returns(false);
        _testCommand.CommandIsPrivateMessageOnly = true;

        // Act
        await _testCommand.Call(_context);

        // Assert
        _action.DidNotReceive().Invoke();
    }

    [Test]
    public async Task Test_Call_ShouldRun_WhenIsPrivateMessageOnlyAndContextIsPrivateMessage()
    {
        // Arrange
        _context.IsPrivateMessage.Returns(true);
        _context.HasSufficientRank(Arg.Any<char>()).Returns(true);
        _testCommand.CommandIsPrivateMessageOnly = true;

        // Act
        await _testCommand.Call(_context);

        // Assert
        _action.Received(1).Invoke();
    }

    [Test]
    public async Task Test_Call_ShouldNotRun_WhenIsWhitelistOnlyAndSenderNotWhitelisted()
    {
        // Arrange
        _context.IsSenderWhitelisted.Returns(false);
        _testCommand.CommandIsWhitelistOnly = true;

        // Act
        await _testCommand.Call(_context);

        // Assert
        _action.DidNotReceive().Invoke();
    }

    [Test]
    public async Task Test_Call_ShouldRun_WhenIsWhitelistOnlyAndSenderIsWhitelisted()
    {
        // Arrange
        _context.IsSenderWhitelisted.Returns(true);
        _context.HasSufficientRank(Arg.Any<char>()).Returns(true);
        _testCommand.CommandIsWhitelistOnly = true;

        // Act
        await _testCommand.Call(_context);

        // Assert
        _action.Received(1).Invoke();
    }

    [Test]
    public async Task Test_Call_ShouldNotRun_WhenInsufficientRank()
    {
        // Arrange
        _context.HasSufficientRank(Arg.Any<char>()).Returns(false);
        _testCommand.CommandRequiredRank = '~';

        // Act
        await _testCommand.Call(_context);

        // Assert
        _action.DidNotReceive().Invoke();
    }

    [Test]
    public async Task Test_Call_ShouldRun_WhenSufficientRank()
    {
        // Arrange
        _context.HasSufficientRank(Arg.Any<char>()).Returns(true);
        _testCommand.CommandRequiredRank = '~';

        // Act
        await _testCommand.Call(_context);

        // Assert
        _action.Received(1).Invoke();
    }

    [Test]
    public void Test_ReplyLocalizedHelpMessage_ShouldReplyLocalizedHelpMessage_WhenCalledWithHelpMessageKey()
    {
        // Arrange
        _testCommand.CommandHelpMessageKey = "Help.Key";
        _context.GetString("Help.Key", Arg.Any<object[]>()).Returns("Localized Help Message");

        // Act
        _testCommand.ReplyLocalizedHelpMessage(_context);

        // Assert
        _context.Received(1).Reply("Localized Help Message");
    }

    private class TestCommand : Command
    {
        private readonly Action _action;

        public TestCommand(Action action)
        {
            _action = action;
        }

        public bool CommandIsPrivateMessageOnly { get; set; }
        public bool CommandIsWhitelistOnly { get; set; }
        public char CommandRequiredRank { get; set; }
        public string CommandHelpMessageKey { get; set; }

        public override bool IsWhitelistOnly => CommandIsWhitelistOnly;
        public override bool IsPrivateMessageOnly => CommandIsPrivateMessageOnly;
        public override char RequiredRank => CommandRequiredRank;
        public override string HelpMessageKey => CommandHelpMessageKey;

        public override Task Run(IContext context)
        {
            _action.Invoke();
            return Task.CompletedTask;
        }
    }
}