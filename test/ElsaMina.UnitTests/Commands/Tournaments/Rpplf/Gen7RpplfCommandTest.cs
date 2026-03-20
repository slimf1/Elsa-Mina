using ElsaMina.Commands.Tournaments.Rpplf;
using ElsaMina.Core.Contexts;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Tournaments.Rpplf;

[TestFixture]
public class Gen7RpplfCommandTest
{
    private IContext _context;
    private Gen7RpplfCommand _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _command = new Gen7RpplfCommand();
    }

    [Test]
    public async Task Test_RunAsync_ShouldCreateGen7UbersTournament()
    {
        await _command.RunAsync(_context);

        _context.Received(1).Reply("/tour create gen7ubers, elim");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSetCorrectTourName()
    {
        await _command.RunAsync(_context);

        _context.Received(1).Reply("/tour name [Gen 7] RPPLF");
    }

    [Test]
    public async Task Test_RunAsync_ShouldRequestGen7RpplfTeams()
    {
        await _command.RunAsync(_context);

        _context.Received(1).Reply("-teams gen7rpplf");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendAutoDq5()
    {
        await _command.RunAsync(_context);

        _context.Received(1).Reply("/tour autodq 5");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendRulesWithZygardeForms()
    {
        var rules = new List<string>();
        _context.When(context => context.Reply(Arg.Is<string>(s => s.StartsWith("/tour rules"))))
            .Do(call => rules.Add(call.Arg<string>()));

        await _command.RunAsync(_context);

        Assert.That(rules, Has.Count.EqualTo(1));
        Assert.That(rules[0], Does.Contain("+Zygarde"));
        Assert.That(rules[0], Does.Contain("+Zygarde-10%"));
        Assert.That(rules[0], Does.Contain("-Zygarde-Complete"));
        Assert.That(rules[0], Does.Contain("-Power Construct"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendRulesWithMegaStonesBanned()
    {
        var rules = new List<string>();
        _context.When(context => context.Reply(Arg.Is<string>(s => s.StartsWith("/tour rules"))))
            .Do(call => rules.Add(call.Arg<string>()));

        await _command.RunAsync(_context);

        Assert.That(rules[0], Does.Contain("-Blazikenite"));
        Assert.That(rules[0], Does.Contain("-Gengarite"));
        Assert.That(rules[0], Does.Contain("-Salamencite"));
    }
}
