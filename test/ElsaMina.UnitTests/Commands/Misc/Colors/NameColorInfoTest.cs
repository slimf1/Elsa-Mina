using ElsaMina.Commands.Misc.Colors;
using ElsaMina.Core.Contexts;
using ElsaMina.Core.Services.CustomColors;
using ElsaMina.Core.Services.Templates;
using ElsaMina.Core.Utils;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Misc.Colors;

public class NameColorInfoTests
{
    private NameColorInfo _nameColorInfo;
    private ITemplatesManager _templatesManager;
    private ICustomColorsManager _customColorsManager;
    private IContext _context;

    [SetUp]
    public void SetUp()
    {
        _templatesManager = Substitute.For<ITemplatesManager>();
        _customColorsManager = Substitute.For<ICustomColorsManager>();
        _context = Substitute.For<IContext>();
        _nameColorInfo = new NameColorInfo(_templatesManager, _customColorsManager);
    }

    [Test]
    public void Test_HelpMessageKey_ShouldBeDefined()
    {
        // Assert
        Assert.That(_nameColorInfo.HelpMessageKey, Is.EqualTo("name_color_help"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendHtml_WhenColorIsResolvedFromCustomMapping()
    {
        // Arrange
        _context.Target.Returns("customUser");
        _customColorsManager.CustomColorsMapping.Returns(new Dictionary<string, string>
        {
            { "customUser", "otherUser" }
        });
        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<NameColorInfoViewModel>())
            .Returns(Task.FromResult("formatted_html"));

        // Act
        await _nameColorInfo.RunAsync(_context);

        // Assert
        await _templatesManager.Received()
            .GetTemplateAsync("Misc/Colors/NameColorInfo", Arg.Is<NameColorInfoViewModel>(vm => vm.Color == "otherUser".ToColor()));
        _context.Received().SendHtmlIn("formatted_html", rankAware: true);
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendHtml_WhenColorIsResolvedFromTargetDirectly()
    {
        // Arrange
        _context.Target.Returns("mec");
        _customColorsManager.CustomColorsMapping.Returns(new Dictionary<string, string>());
        _templatesManager.GetTemplateAsync(Arg.Any<string>(), Arg.Any<NameColorInfoViewModel>())
            .Returns(Task.FromResult("formatted_html"));

        // Act
        await _nameColorInfo.RunAsync(_context);

        // Assert
        await _templatesManager.Received()
            .GetTemplateAsync("Misc/Colors/NameColorInfo", Arg.Is<NameColorInfoViewModel>(vm => vm.Color == "mec".ToColor()));
        _context.Received().SendHtmlIn("formatted_html", rankAware: true);
    }
}
