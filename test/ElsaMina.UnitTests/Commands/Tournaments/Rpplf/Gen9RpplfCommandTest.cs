using ElsaMina.Commands.Tournaments.Rpplf;
using ElsaMina.Core.Contexts;
using NSubstitute;

namespace ElsaMina.UnitTests.Commands.Tournaments.Rpplf;

[TestFixture]
public class Gen9RpplfCommandTest
{
    private IContext _context;
    private Gen9RpplfCommand _command;

    [SetUp]
    public void SetUp()
    {
        _context = Substitute.For<IContext>();
        _command = new Gen9RpplfCommand();
    }

    [Test]
    public async Task Test_RunAsync_ShouldCreateGen9UbersTournament()
    {
        await _command.RunAsync(_context);

        _context.Received(1).Reply("/tour create gen9ubers, elim");
    }

    [Test]
    public async Task Test_RunAsync_ShouldSetCorrectTourName()
    {
        await _command.RunAsync(_context);

        _context.Received(1).Reply("/tour name [Gen 9] RPPLF");
    }

    [Test]
    public async Task Test_RunAsync_ShouldRequestGen9RpplfTeams()
    {
        await _command.RunAsync(_context);

        _context.Received(1).Reply("-teams gen9rpplf");
    }

    [Test]
    public async Task Test_RunAsync_ShouldNotSendAutoDq()
    {
        await _command.RunAsync(_context);

        _context.DidNotReceive().Reply(Arg.Is<string>(s => s.StartsWith("/tour autodq")));
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendRulesWithItemClause()
    {
        var rules = new List<string>();
        _context.When(context => context.Reply(Arg.Is<string>(s => s.StartsWith("/tour rules"))))
            .Do(call => rules.Add(call.Arg<string>()));

        await _command.RunAsync(_context);

        Assert.That(rules, Has.Count.EqualTo(1));
        Assert.That(rules[0], Does.Contain("Item Clause=1"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendRulesWithRazorFangAllowed()
    {
        var rules = new List<string>();
        _context.When(context => context.Reply(Arg.Is<string>(s => s.StartsWith("/tour rules"))))
            .Do(call => rules.Add(call.Arg<string>()));

        await _command.RunAsync(_context);

        Assert.That(rules[0], Does.Contain("+Razor Fang"));
    }

    [Test]
    public async Task Test_RunAsync_ShouldSendRulesWithTerapagosBanned()
    {
        var rules = new List<string>();
        _context.When(context => context.Reply(Arg.Is<string>(s => s.StartsWith("/tour rules"))))
            .Do(call => rules.Add(call.Arg<string>()));

        await _command.RunAsync(_context);

        Assert.That(rules[0], Does.Contain("-Terapagos-Stellar"));
        Assert.That(rules[0], Does.Contain("+Terapagos"));
    }
}
