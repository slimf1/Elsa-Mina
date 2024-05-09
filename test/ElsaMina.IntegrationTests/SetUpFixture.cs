using ElsaMina.Core;
using NSubstitute;
using Serilog;

namespace ElsaMina.IntegrationTests;

[SetUpFixture]
public class SetUpFixture
{
    [OneTimeSetUp]
    public void OnTimeSetup()
    {
        Logger.Current = Substitute.For<ILogger>();
    }
}