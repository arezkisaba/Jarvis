using NUnit.Framework;

namespace Jarvis.Features.Services.TorrentScrapperService.Models;

[TestFixture]
public class TorrentDtoTU
{
    [TestCase(TestName = "TorrentDto s'initialise correctement")]
    public void TorrentDto_Construct1_TestCase()
    {
        var obj = new TorrentDto(
            DescriptionUrl: "descriptionUrl",
            Name: "name",
            Provider: "provider",
            Seeds: 17,
            Size: 699
        );
        Assert.AreEqual("descriptionUrl", obj.DescriptionUrl);
        Assert.AreEqual("name", obj.Name);
        Assert.AreEqual("provider", obj.Provider);
        Assert.AreEqual(17, obj.Seeds);
        Assert.AreEqual(699, obj.Size);
    }
}
