using NUnit.Framework;

namespace Jarvis.Features.BackgroundAgents.MediaStorageBackgroundAgent.Models;

[TestFixture]
public class MediaStorageStateModelUnitTests
{
    [TestCase(TestName = "MediaStorageStateModel s'initialise correctement")]
    public void MediaStorageStateModel_Construct1_TestCase()
    {
        ////var obj = new MediaStorageStateModel(
        ////    availableSpaceSize: 10,
        ////    usedSpaceSize: 11,
        ////    moviesFolderSize: 123,
        ////    tvShowsFolderSize: 456);
        ////Assert.AreEqual(10, obj.AvailableSpaceSize);
        ////Assert.AreEqual(11, obj.UsedSpaceSize);
        ////Assert.AreEqual(123, obj.MoviesFolderSize);
        ////Assert.AreEqual(456, obj.TvShowsFolderSize);
    }
}
