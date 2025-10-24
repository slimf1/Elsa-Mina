using ElsaMina.Core.Utils;

namespace ElsaMina.UnitTests.Core.Utils;

public class DoubleExtensionsTest
{
    [Test]
    [TestCase(1.0000, 1.00005, 1e-3, ExpectedResult = true)]
    [TestCase(1.0000, 1.002, 1e-3, ExpectedResult = false)]
    [TestCase(-5.0, -5.000001, 1e-5, ExpectedResult = true)]
    public bool Test_IsApproximatelyEqualTo_ShouldReturnTrueForCloseValues(double value1, double value2, double tolerance)
    {
        // Act & Assert
        return value1.IsApproximatelyEqualTo(value2, tolerance);
    }
}