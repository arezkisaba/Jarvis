using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Jarvis.Features.Services.TorrentScrapperService.Providers;

[TestFixture]
[ExcludeFromCodeCoverage]
public class ZeTorrentsTorrentScrapperServiceTI
{
    [TestCase(TestName = "GetResultsAsync renvois une liste de torrents")]
    public async Task GetResultsAsync_TestCase()
    {
        var torrentScrapperService = new ZeTorrentsTorrentScrapperService("https://www.zetorrents.nl");
        var items = await torrentScrapperService.GetResultsAsync("game of thrones saison 8 french");
        Assert.IsTrue(items != null && items.Any());
    }
}
