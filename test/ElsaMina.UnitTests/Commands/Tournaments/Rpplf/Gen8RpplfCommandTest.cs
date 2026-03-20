using ElsaMina.Commands.Tournaments.Rpplf;
using ElsaMina.Core.Contexts;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Tournaments.Rpplf;

[TestFixture]
public class Gen8RpplfCommandTest
{
    private IContext _context;
    private Gen8RpplfCommand _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _command = new Gen8RpplfCommand();
    }

    [Test]
    public async Task Test_RunAsync_ShouldCreateGen8UbersTournament()
    {
        await _command.RunAsync(_context);

        _context.Received(1).Reply("/tour create gen8ubers, elim");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSetCorrectTourName()
    {
        await _command.RunAsync(_context);

        _context.Received(1).Reply("/tour name [Gen 8] RPPLF");
    }

    [Test]
    public async Task Test_RunAsync_ShouldRequestGen8RpplfTeams()
    {
        await _command.RunAsync(_context);

        _context.Received(1).Reply("-teams gen8rpplf");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendAutoDq5()
    {
        await _command.RunAsync(_context);

        _context.Received(1).Reply("/tour autodq 5");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendRulesWithNoDynamaxClause()
    {
        var rules = new List<string>();
        _context.When(context => context.Reply(Arg.Is<string>(s => s.StartsWith("/tour rules"))))
            .Do(call => rules.Add(call.Arg<string>()));

        await _command.RunAsync(_context);

        Assert.That(rules, Has.Count.EqualTo(1));
        Assert.That(rules[0], Does.Contain("!Dynamax Clause"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendRulesWithUrshifuRapidStrikeAllowed()
    {
        var rules = new List<string>();
        _context.When(context => context.Reply(Arg.Is<string>(s => s.StartsWith("/tour rules"))))
            .Do(call => rules.Add(call.Arg<string>()));

        await _command.RunAsync(_context);

        Assert.That(rules[0], Does.Contain("+Urshifu-Rapid-Strike"));
    }
}
