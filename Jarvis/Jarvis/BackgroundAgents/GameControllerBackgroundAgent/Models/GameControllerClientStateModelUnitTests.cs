using NUnit.Framework;

namespace Jarvis.UnitTests;

[TestFixture]
public class GameControllerClientStateModelUnitTests
{
    [TestCase(TestName = "GameControllerClientStateModel s'initialise correctement")]
    public void GameControllerClientStateModel_Construct1_TestCase()
    {
        var obj = new GameControllerClientStateModel(
            title: "Game controller status",
            subtitle: "Xbox controller",
            isActive: true);
        Assert.AreEqual("Game controller status", obj.Title);
        Assert.AreEqual("Xbox controller", obj.Subtitle);
        Assert.AreEqual(true, obj.IsActive);
    }
}
