using NUnit.Framework;

namespace Jarvis.Features.BackgroundAgents.MediaStorageBackgroundAgent.MediaStorageService;

[TestFixture]
public class MediaStorageServiceUnitTest
{
    [TestCase(TestName = "GetFolderContentAndSize retourne une liste de dossier")]
    public void GetFolderContentAndSize_NotEmpty_TestCase()
    {
        var mediaStorageService = new MediaStorageService();
        var result = mediaStorageService.GetFolderContentAndSize(@"D:\NVIDIA_SHIELD");
        Assert.IsTrue(result.Any());
    }

    [TestCase(TestName = "GetDriveInformations retourne des informations")]
    public void GetDriveInformations_HasInformations_TestCase()
    {
        var mediaStorageService = new MediaStorageService();
        var (UsedSpace, FreeSpace, TotalSpace) = mediaStorageService.GetDriveInformations(@"D:\");
        Assert.IsTrue(UsedSpace > 0);
        Assert.IsTrue(FreeSpace > 0);
        Assert.IsTrue(TotalSpace > 0);
    }
}
