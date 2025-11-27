using System.Globalization;
using ElsaMina.Commands.Arcade;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Rooms;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess;
using ElsaMina.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Arcade;

public class DisplayArcadeLevelsTests
{
    private DisplayArcadeLevels _command;
    private ITemplatesManager _templatesManager;
    private IBotDbContextFactory _dbContextFactory;
    private BotDbContext _dbContext;
    private IContext _context;
    private DbSet<ArcadeLevel> _arcadeLevelsDbSet;

    [SetUp]
    public void SetUp()
    {
        _templatesManager = Substitute.For<ITemplatesManager>();
        _dbContextFactory = Substitute.For<IBotDbContextFactory>();
        _dbContext = Substitute.For<BotDbContext>();
        _context = Substitute.For<IContext>();
        _arcadeLevelsDbSet = Substitute.For<DbSet<ArcadeLevel>>();

        _dbContext.ArcadeLevels.Returns(_arcadeLevelsDbSet);
        _dbContextFactory.CreateDbContextAsync(Arg.Any<CancellationToken>())
            .Returns(_dbContext);

        _command = new DisplayArcadeLevels(_templatesManager, _dbContextFactory);
    }

    [Test]
    public void Test_RequiredRank_ShouldBeRegular()
    {
        // Assert
        Assert.That(_command.RequiredRank, Is.EqualTo(Rank.Regular));
    }

    [Test]
    public void Test_RoomRestriction_ShouldContainArcadeAndBotDevelopment()
    {
        // Assert
        Assert.That(_command.RoomRestriction, Is.EqualTo(new[] { "arcade", "botdevelopment" }));
    }

    [Test]
    public void Test_HelpMessageKey_ShouldReturnCorrectKey()
    {
        // Assert
        Assert.That(_command.HelpMessageKey, Is.EqualTo("display_paliers_help"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendHtml_WhenLevelsArePresent()
    {
        // Arrange
        var culture = new CultureInfo("en-US");
        _context.Culture.Returns(culture);
        var levels = new List<ArcadeLevel>
        {
            new() { Id = "user1", Level = 3 },
            new() { Id = "user2", Level = 3 },
            new() { Id = "user3", Level = 2 }
        };

        SetupDbSetToReturnLevels(levels);

        var expectedTemplate = "<html>Rendered Content</html>";
        _templatesManager.GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Is<ArcadeLevelsViewModel>(vm =>
                vm.Culture == culture &&
                vm.Levels[3].Count == 2 &&
                vm.Levels[2].Count == 1))
            .Returns(Task.FromResult(expectedTemplate));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyHtml(expectedTemplate.RemoveNewlines(), rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendEmptyMessage_WhenNoLevelsArePresent()
    {
        // Arrange
        SetupDbSetToReturnLevels(new List<ArcadeLevel>());

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("arcade_level_no_users");
        await _templatesManager.DidNotReceive().GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleException_WhenDbContextThrowsError()
    {
        // Arrange
        _arcadeLevelsDbSet.When(x => x.GetAsyncEnumerator(Arg.Any<CancellationToken>()))
            .Do(x => throw new Exception("Database error"));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyLocalizedMessage("arcade_level_no_users");
        await _templatesManager.DidNotReceive().GetTemplateAsync(Arg.Any<string>(), Arg.Any<object>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldGroupLevelsByNumber_WhenMultipleLevelsExist()
    {
        // Arrange
        var culture = new CultureInfo("en-US");
        _context.Culture.Returns(culture);
        var levels = new List<ArcadeLevel>
        {
            new() { Id = "user1", Level = 5 },
            new() { Id = "user2", Level = 5 },
            new() { Id = "user3", Level = 5 },
            new() { Id = "user4", Level = 3 },
            new() { Id = "user5", Level = 3 },
            new() { Id = "user6", Level = 1 }
        };

        SetupDbSetToReturnLevels(levels);

        var expectedTemplate = "<html>Content</html>";
        _templatesManager.GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Any<ArcadeLevelsViewModel>())
            .Returns(Task.FromResult(expectedTemplate));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Is<ArcadeLevelsViewModel>(vm =>
                vm.Levels.Count == 3 &&
                vm.Levels[5].Count == 3 &&
                vm.Levels[3].Count == 2 &&
                vm.Levels[1].Count == 1));
    }

    [Test]
    public async Task Test_RunAsync_ShouldPassCorrectCulture_ToViewModel()
    {
        // Arrange
        var culture = new CultureInfo("fr-FR");
        _context.Culture.Returns(culture);
        var levels = new List<ArcadeLevel>
        {
            new() { Id = "user1", Level = 1 }
        };

        SetupDbSetToReturnLevels(levels);

        var expectedTemplate = "<html>Content</html>";
        _templatesManager.GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Any<ArcadeLevelsViewModel>())
            .Returns(Task.FromResult(expectedTemplate));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Is<ArcadeLevelsViewModel>(vm => vm.Culture == culture));
    }

    [Test]
    public async Task Test_RunAsync_ShouldRemoveNewlines_FromTemplateBeforeSending()
    {
        // Arrange
        var culture = new CultureInfo("en-US");
        _context.Culture.Returns(culture);
        var levels = new List<ArcadeLevel>
        {
            new() { Id = "user1", Level = 1 }
        };

        SetupDbSetToReturnLevels(levels);

        var templateWithNewlines = "<html>\n<body>\r\nContent\r\n</body>\n</html>";
        _templatesManager.GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Any<ArcadeLevelsViewModel>())
            .Returns(Task.FromResult(templateWithNewlines));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyHtml(
            Arg.Is<string>(html => !html.Contains("\n") && !html.Contains("\r")),
            rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldSetRankAwareToTrue_WhenSendingHtml()
    {
        // Arrange
        var culture = new CultureInfo("en-US");
        _context.Culture.Returns(culture);
        var levels = new List<ArcadeLevel>
        {
            new() { Id = "user1", Level = 1 }
        };

        SetupDbSetToReturnLevels(levels);

        var expectedTemplate = "<html>Content</html>";
        _templatesManager.GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Any<ArcadeLevelsViewModel>())
            .Returns(Task.FromResult(expectedTemplate));

        // Act
        await _command.RunAsync(_context);

        // Assert
        _context.Received(1).ReplyHtml(Arg.Any<string>(), rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldUseCorrectTemplatePath()
    {
        // Arrange
        var culture = new CultureInfo("en-US");
        _context.Culture.Returns(culture);
        var levels = new List<ArcadeLevel>
        {
            new() { Id = "user1", Level = 1 }
        };

        SetupDbSetToReturnLevels(levels);

        var expectedTemplate = "<html>Content</html>";
        _templatesManager.GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Any<ArcadeLevelsViewModel>())
            .Returns(Task.FromResult(expectedTemplate));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Any<ArcadeLevelsViewModel>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldPassCancellationToken_ToDbContext()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var culture = new CultureInfo("en-US");
        _context.Culture.Returns(culture);
        var levels = new List<ArcadeLevel>
        {
            new() { Id = "user1", Level = 1 }
        };

        SetupDbSetToReturnLevels(levels);

        var expectedTemplate = "<html>Content</html>";
        _templatesManager.GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Any<ArcadeLevelsViewModel>())
            .Returns(Task.FromResult(expectedTemplate));

        // Act
        await _command.RunAsync(_context, cancellationToken);

        // Assert
        await _dbContextFactory.Received(1).CreateDbContextAsync(cancellationToken);
    }

    [Test]
    public async Task Test_RunAsync_ShouldPreserveLevelUserIds_InViewModel()
    {
        // Arrange
        var culture = new CultureInfo("en-US");
        _context.Culture.Returns(culture);
        var levels = new List<ArcadeLevel>
        {
            new() { Id = "alice", Level = 2 },
            new() { Id = "bob", Level = 2 }
        };

        SetupDbSetToReturnLevels(levels);

        var expectedTemplate = "<html>Content</html>";
        _templatesManager.GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Any<ArcadeLevelsViewModel>())
            .Returns(Task.FromResult(expectedTemplate));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Is<ArcadeLevelsViewModel>(vm =>
                vm.Levels[2].Contains("alice") &&
                vm.Levels[2].Contains("bob")));
    }

    [Test]
    public async Task Test_RunAsync_ShouldDisposeDbContext_AfterExecution()
    {
        // Arrange
        var levels = new List<ArcadeLevel>
        {
            new() { Id = "user1", Level = 1 }
        };

        SetupDbSetToReturnLevels(levels);

        var expectedTemplate = "<html>Content</html>";
        _templatesManager.GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Any<ArcadeLevelsViewModel>())
            .Returns(Task.FromResult(expectedTemplate));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContext.Received(1).DisposeAsync();
    }

    [Test]
    public async Task Test_RunAsync_ShouldCreateDbContext_ViaFactory()
    {
        // Arrange
        var levels = new List<ArcadeLevel>
        {
            new() { Id = "user1", Level = 1 }
        };

        SetupDbSetToReturnLevels(levels);

        var expectedTemplate = "<html>Content</html>";
        _templatesManager.GetTemplateAsync(
            Arg.Any<string>(),
            Arg.Any<ArcadeLevelsViewModel>())
            .Returns(Task.FromResult(expectedTemplate));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _dbContextFactory.Received(1).CreateDbContextAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleSingleUserInLevel()
    {
        // Arrange
        var culture = new CultureInfo("en-US");
        _context.Culture.Returns(culture);
        var levels = new List<ArcadeLevel>
        {
            new() { Id = "singleuser", Level = 10 }
        };

        SetupDbSetToReturnLevels(levels);

        var expectedTemplate = "<html>Content</html>";
        _templatesManager.GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Any<ArcadeLevelsViewModel>())
            .Returns(Task.FromResult(expectedTemplate));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Is<ArcadeLevelsViewModel>(vm =>
                vm.Levels.Count == 1 &&
                vm.Levels[10].Count == 1 &&
                vm.Levels[10].Contains("singleuser")));
    }

    [Test]
    public async Task Test_RunAsync_ShouldHandleMultipleDifferentLevels()
    {
        // Arrange
        var culture = new CultureInfo("en-US");
        _context.Culture.Returns(culture);
        var levels = new List<ArcadeLevel>
        {
            new() { Id = "user1", Level = 1 },
            new() { Id = "user2", Level = 2 },
            new() { Id = "user3", Level = 3 },
            new() { Id = "user4", Level = 4 },
            new() { Id = "user5", Level = 5 }
        };

        SetupDbSetToReturnLevels(levels);

        var expectedTemplate = "<html>Content</html>";
        _templatesManager.GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Any<ArcadeLevelsViewModel>())
            .Returns(Task.FromResult(expectedTemplate));

        // Act
        await _command.RunAsync(_context);

        // Assert
        await _templatesManager.Received(1).GetTemplateAsync(
            "Arcade/ArcadeLevels",
            Arg.Is<ArcadeLevelsViewModel>(vm =>
                vm.Levels.Count == 5 &&
                vm.Levels.All(kvp => kvp.Value.Count == 1)));
    }

    private void SetupDbSetToReturnLevels(List<ArcadeLevel> levels)
    {
        var queryable = levels.AsQueryable();
        var asyncEnumerable = new TestAsyncEnumerable<ArcadeLevel>(queryable);

        _arcadeLevelsDbSet.AsQueryable().Returns(queryable);
        ((IAsyncEnumerable<ArcadeLevel>)_arcadeLevelsDbSet).GetAsyncEnumerator(Arg.Any<CancellationToken>())
            .Returns(asyncEnumerable.GetAsyncEnumerator());
    }

    // Helper classes for async enumeration
    private class TestAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;

        public TestAsyncEnumerable(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(_enumerable.GetEnumerator());
        }
    }

    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public TestAsyncEnumerator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public T Current => _enumerator.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_enumerator.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            _enumerator.Dispose();
            return new ValueTask();
        }
    }
}