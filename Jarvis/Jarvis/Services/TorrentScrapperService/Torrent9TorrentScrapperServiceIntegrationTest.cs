using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Jarvis.IntegrationTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public class Torrent9TorrentScrapperServiceIntegrationTest
{
    [TestCase(TestName = "GetStringAsync renvois du contenu HTML")]
    public async Task GetAsync_TestCase()
    {
        var torrentScrapperService = new Torrent9TorrentScrapperService("https://www.torrent9.nl");
        var response = await torrentScrapperService.GetStringAsync("game of thrones saison 8 french");
        Assert.IsTrue(!string.IsNullOrWhiteSpace(response));
    }

    [TestCase(TestName = "GetTorrentsFromHtml renvois une liste de torrents")]
    public async Task GetTorrentsFromHtml_TestCase()
    {
        var torrentScrapperService = new Torrent9TorrentScrapperService("https://www.torrent9.nl");
        var response = await torrentScrapperService.GetStringAsync("game of thrones saison 8 french");
        var items = torrentScrapperService.GetTorrentsFromHtml(response);
        Assert.IsTrue(items != null && items.Any());
    }
}
