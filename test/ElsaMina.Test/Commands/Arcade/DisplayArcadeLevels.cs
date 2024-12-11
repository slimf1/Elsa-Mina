using System.Globalization;
using ElsaMina.Commands.Arcade;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ElsaMina.Test.Commands.Arcade;

public class DisplayArcadeLevelsTests
{
    private DisplayArcadeLevels _command;
    private IArcadeLevelRepository _arcadeLevelRepository;
    private ITemplatesManager _templatesManager;

    [SetUp]
    public void SetUp()
    {
        _arcadeLevelRepository = Substitute.For<IArcadeLevelRepository>();
        _templatesManager = Substitute.For<ITemplatesManager>();
        _command = new DisplayArcadeLevels(_arcadeLevelRepository, _templatesManager);
    }

    [Test]
    public async Task Test_Run_ShouldSendHtml_WhenLevelsArePresent()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        var culture = new CultureInfo("en-US");
        context.Culture.Returns(culture);
        var levels = new List<ArcadeLevel>
        {
            new() { Id = "user1", Level = 3 },
            new() { Id = "user2", Level = 3 },
            new() { Id = "user3", Level = 2 }
        };

        _arcadeLevelRepository.GetAllAsync().Returns(levels);

        var expectedTemplate = "<html>Rendered Content</html>";
        _templatesManager.GetTemplate(
            "Arcade/ArcadeLevels",
            Arg.Is<ArcadeLevelsViewModel>(vm =>
                vm.Culture == culture &&
                vm.Levels[3].Count == 2 &&
                vm.Levels[2].Count == 1))
            .Returns(Task.FromResult(expectedTemplate));

        // Act
        await _command.Run(context);

        // Assert
        context.Received(1).SendHtml(expectedTemplate.RemoveNewlines(), rankAware: true);
    }

    [Test]
    public async Task Test_Run_ShouldSendEmptyMessage_WhenNoLevelsArePresent()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        _arcadeLevelRepository.GetAllAsync().Returns(new List<ArcadeLevel>());

        // Act
        await _command.Run(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("arcade_level_no_users");
    }

    [Test]
    public async Task Test_Run_ShouldHandleException_WhenRepositoryThrowsError()
    {
        // Arrange
        var context = Substitute.For<IContext>();
        _arcadeLevelRepository.GetAllAsync().Throws(new Exception("Database error"));

        // Act
        await _command.Run(context);

        // Assert
        context.Received(1).ReplyLocalizedMessage("arcade_level_no_users");
    }
}
