using NUnit.Framework;

namespace Jarvis.BackgroundAgents.TorrentClientBackgroundAgent.Models;

[TestFixture]
public class TorrentClientStateModelUnitTests
{
    [TestCase(TestName = "TorrentClientStateModel s'initialise correctement")]
    public void TorrentClientStateModel_Construct1_TestCase()
    {
        var obj = new TorrentClientStateModel(
            title: "Torrent client status",
            subtitle: "Transmission",
            isActive: true);
        Assert.AreEqual("Torrent client status", obj.Title);
        Assert.AreEqual("Transmission", obj.Subtitle);
        Assert.AreEqual(true, obj.IsActive);
    }
}
