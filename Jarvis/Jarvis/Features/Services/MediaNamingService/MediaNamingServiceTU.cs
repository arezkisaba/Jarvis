using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Jarvis.Features.Services.MediaNamingService;

[TestFixture]
[ExcludeFromCodeCoverage]
public class MediaNamingServiceTU
{
    [TestCase(TestName = "GetDisplayNameForMovie : Cas 1")]
    public void GetDisplayNameForMovie_TestCase_1()
    {
        var mediaNamingService = new MediaNamingService();
        var displayName = mediaNamingService.GetDisplayNameForMovie("Avengers : L'Ère d'Ultron FRENCH HDTV 2021");
        Assert.AreEqual("Avengers : L'Ère d'Ultron FRENCH HDTV 2021", displayName);
    }

    [TestCase(TestName = "GetDisplayNameForEpisode : Cas 1")]
    public void GetDisplayNameForEpisode_TestCase_1()
    {
        var mediaNamingService = new MediaNamingService();
        var displayName = mediaNamingService.GetDisplayNameForEpisode("Visitors", 1, 7);
        Assert.AreEqual("Visitors S01E07", displayName);
    }

    [TestCase(TestName = "GetPossibleMovieTitles : Cas 1")]
    public void GetPossibleMovieTitles_TestCase_1()
    {
        var mediaNamingService = new MediaNamingService();
        var titles = mediaNamingService.GetPossibleMediaTitles("Avengers : L'Ère d'Ultron FRENCH HDTV 2021");
        Assert.AreEqual(7, titles.Count());
        Assert.AreEqual("Avengers : L'Ère d'Ultron FRENCH HDTV 2021", titles[0]);
        Assert.AreEqual("Avengers : L'Ère d'Ultron FRENCH HDTV", titles[1]);
        Assert.AreEqual("Avengers : L'Ère d'Ultron FRENCH", titles[2]);
        Assert.AreEqual("Avengers : L'Ère d'Ultron", titles[3]);
        Assert.AreEqual("Avengers : L'Ère", titles[4]);
        Assert.AreEqual("Avengers :", titles[5]);
        Assert.AreEqual("Avengers", titles[6]);
    }
}
