using ElsaMina.Logging;
using NSubstitute;

namespace ElsaMina.IntegrationTests;

[SetUpFixture]
public class SetUpFixture
{
    [OneTimeSetUp]
    public void SetUp()
    {
        Log.Configuration = Substitute.For<ILoggingConfiguration>();
    }
}