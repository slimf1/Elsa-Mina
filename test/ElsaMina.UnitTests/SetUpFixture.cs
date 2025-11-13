using ElsaMina.Logging;
using NSubstitute;

namespace ElsaMina.UnitTests;

[SetUpFixture]
public class SetUpFixture
{
    [OneTimeSetUp]
    public void SetUp()
    {
        Log.Configuration = Substitute.For<ILoggingConfiguration>();
    }
}