using ElsaMina.Core;
using NSubstitute;
using Serilog;

namespace ElsaMina.Test;

[SetUpFixture]
public class SetupFixture
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Logger.Current = Substitute.For<ILogger>();
    }
}