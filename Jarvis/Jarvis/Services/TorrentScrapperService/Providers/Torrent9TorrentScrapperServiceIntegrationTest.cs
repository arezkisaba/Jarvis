using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Jarvis.Services.TorrentScrapperService.Providers;

[TestFixture]
[ExcludeFromCodeCoverage]
public class Torrent9TorrentScrapperServiceIntegrationTest
{
    [TestCase(TestName = "GetResultsAsync renvois une liste de torrents")]
    public async Task GetResultsAsync_TestCase()
    {
        var torrentScrapperService = new Torrent9TorrentScrapperService("https://www.torrent9.nl");
        var items = await torrentScrapperService.GetResultsAsync("game of thrones saison 8 french");
        Assert.IsTrue(items != null && items.Any());
    }
}
