using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Jarvis.Services.TorrentScrapperService.Providers;

[TestFixture]
[ExcludeFromCodeCoverage]
public class GkTorrentTorrentScrapperServiceIntegrationTest
{
    [TestCase(TestName = "GetResultsAsync renvois une liste de torrents")]
    public async Task GetResultsAsync_TestCase()
    {
        var torrentScrapperService = new GkTorrentTorrentScrapperService("https://www.gktorrents.org");
        var items = await torrentScrapperService.GetResultsAsync("game of thrones saison 8 french");
        Assert.IsTrue(items != null && items.Any());
    }
}
