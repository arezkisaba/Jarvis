using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Jarvis.Features.Services.TorrentScrapperService.Providers;

[TestFixture]
[ExcludeFromCodeCoverage]
public class TorrentDownloadsTorrentScrapperServiceTI
{
    [TestCase(TestName = "GetResultsAsync renvois une liste de torrents")]
    public async Task GetResultsAsync_TestCase()
    {
        var torrentScrapperService = new TorrentDownloadsTorrentScrapperService("https://torrentdownloads.d4.re");
        var items = await torrentScrapperService.GetResultsAsync("game of thrones saison 8 french");
        Assert.IsTrue(items != null && items.Any());
    }
}
