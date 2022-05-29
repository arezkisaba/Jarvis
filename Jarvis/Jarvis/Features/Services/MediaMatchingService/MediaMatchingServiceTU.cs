using Jarvis.Features.Services.MediaMatchingService.Models;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Jarvis.Features.Services.MediaMatchingService;

[TestFixture]
[ExcludeFromCodeCoverage]
public class MediaMatchingServiceTU
{
    [TestCase(TestName = "GetMediaTypeAndInformations déduit l'épisode : Cas 1")]
    public void GetMediaTypeAndInformations_Episode_TestCase_1()
    {
        var mediaMatchingService = new MediaMatchingService();
        var (mediaType, match) = mediaMatchingService.GetMediaTypeAndInformations("Visitors S01E07 FRENCH HDTV");
        Assert.AreEqual(MediaTypeModel.Episode, mediaType);
        Assert.AreEqual("Visitors", match.Groups[1].Value);
        Assert.AreEqual(1, int.Parse(match.Groups[2].Value));
        Assert.AreEqual(7, int.Parse(match.Groups[3].Value));
    }

    [TestCase(TestName = "GetMediaTypeAndInformations déduit l'épisode : Cas 2")]
    public void GetMediaTypeAndInformations_Episode_TestCase_2()
    {
        var mediaMatchingService = new MediaMatchingService();
        var (mediaType, match) = mediaMatchingService.GetMediaTypeAndInformations("Visitors Saison 01 Episode 07 FRENCH HDTV");
        Assert.AreEqual(MediaTypeModel.Episode, mediaType);
        Assert.AreEqual("Visitors", match.Groups[1].Value);
        Assert.AreEqual(1, int.Parse(match.Groups[2].Value));
        Assert.AreEqual(7, int.Parse(match.Groups[3].Value));
    }

    [TestCase(TestName = "GetMediaTypeAndInformations déduit l'épisode : Cas 3")]
    public void GetMediaTypeAndInformations_Episode_TestCase_3()
    {
        var mediaMatchingService = new MediaMatchingService();
        var (mediaType, match) = mediaMatchingService.GetMediaTypeAndInformations("Visitors Saison 1 Episode 7 FRENCH HDTV");
        Assert.AreEqual(MediaTypeModel.Episode, mediaType);
        Assert.AreEqual("Visitors", match.Groups[1].Value);
        Assert.AreEqual(1, int.Parse(match.Groups[2].Value));
        Assert.AreEqual(7, int.Parse(match.Groups[3].Value));
    }

    [TestCase(TestName = "GetMediaTypeAndInformations déduit la saison : Cas 1")]
    public void GetMediaTypeAndInformations_Saison_TestCase_1()
    {
        var mediaMatchingService = new MediaMatchingService();
        var (mediaType, match) = mediaMatchingService.GetMediaTypeAndInformations("Visitors S01 FRENCH HDTV");
        Assert.AreEqual(MediaTypeModel.Season, mediaType);
        Assert.AreEqual("Visitors", match.Groups[1].Value);
        Assert.AreEqual(1, int.Parse(match.Groups[2].Value));
    }

    [TestCase(TestName = "GetMediaTypeAndInformations déduit la saison : Cas 2")]
    public void GetMediaTypeAndInformations_Saison_TestCase_2()
    {
        var mediaMatchingService = new MediaMatchingService();
        var (mediaType, match) = mediaMatchingService.GetMediaTypeAndInformations("Visitors Saison 01 FRENCH HDTV");
        Assert.AreEqual(MediaTypeModel.Season, mediaType);
        Assert.AreEqual("Visitors", match.Groups[1].Value);
        Assert.AreEqual(1, int.Parse(match.Groups[2].Value));
    }

    [TestCase(TestName = "GetMediaTypeAndInformations déduit la saison : Cas 3")]
    public void GetMediaTypeAndInformations_Saison_TestCase_3()
    {
        var mediaMatchingService = new MediaMatchingService();
        var (mediaType, match) = mediaMatchingService.GetMediaTypeAndInformations("Visitors Saison 1 FRENCH HDTV");
        Assert.AreEqual(MediaTypeModel.Season, mediaType);
        Assert.AreEqual("Visitors", match.Groups[1].Value);
        Assert.AreEqual(1, int.Parse(match.Groups[2].Value));
    }

    [TestCase(TestName = "GetPossibleMovieTitles : Cas 1")]
    public void GetPossibleMovieTitles_TestCase_1()
    {
        var mediaMatchingService = new MediaMatchingService();
        var titles = mediaMatchingService.GetPossibleMovieTitles("Avengers : L'Ère d'Ultron FRENCH HDTV 2021");
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
