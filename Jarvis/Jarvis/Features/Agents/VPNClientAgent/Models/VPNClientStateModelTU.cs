using NUnit.Framework;

namespace Jarvis.Features.Agents.VPNClientAgent.Models;

[TestFixture]
public class VPNClientStateModelTU
{
    [TestCase(TestName = "VPNClientStateModel s'initialise correctement")]
    public void VPNClientStateModel_Construct1_TestCase()
    {
        var obj = new VPNClientStateModel(
            title: "Torrent client status",
            subtitle: "Transmission",
            isActive: true);
        Assert.AreEqual("Torrent client status", obj.Title);
        Assert.AreEqual("Transmission", obj.Subtitle);
        Assert.AreEqual(true, obj.IsActive);
    }
}
