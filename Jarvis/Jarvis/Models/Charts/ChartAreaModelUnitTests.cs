using NUnit.Framework;

namespace Jarvis.UnitTests;

[TestFixture]
public class ChartAreaModelUnitTests
{
    [TestCase(TestName = "ChartAreaModel s'initialise correctement")]
    public void ChartAreaModel_Construct1_TestCase()
    {
        var obj = new ChartAreaModel(
            displayName: "iPhone",
            value: 13,
            backgroundColor: "#ff0000");
        Assert.AreEqual("iPhone", obj.DisplayName);
        Assert.AreEqual(13, obj.Value);
        Assert.AreEqual("#ff0000", obj.BackgroundColor);
    }
}
