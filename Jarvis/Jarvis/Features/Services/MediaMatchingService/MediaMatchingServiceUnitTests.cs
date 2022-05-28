using Jarvis.Features.Services.MediaMatchingService.Models;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Jarvis.Features.Services.MediaMatchingService;

[TestFixture]
[ExcludeFromCodeCoverage]
public class MediaMatchingServiceUnitTests
{
    [TestCase(TestName = "GetMediaTypeAndInformations déduit l'épisode : Cas 1")]
    public void GetMediaTypeAndInformations_Episode_TestCase_1()
    {
        var mediaMatchingService = new MediaMatchingService();
        var (mediaType, match) = mediaMatchingService.GetMediaTypeAndInformations("Visitors S01E07");
        Assert.AreEqual(MediaTypeModel.Episode, mediaType);
        Assert.AreEqual("Visitors", match.Groups[1].Value);
        Assert.AreEqual(1, int.Parse(match.Groups[2].Value));
        Assert.AreEqual(7, int.Parse(match.Groups[3].Value));
    }

    [TestCase(TestName = "GetMediaTypeAndInformations déduit l'épisode : Cas 2")]
    public void GetMediaTypeAndInformations_Episode_TestCase_2()
    {
        var mediaMatchingService = new MediaMatchingService();
        var (mediaType, match) = mediaMatchingService.GetMediaTypeAndInformations("Visitors Saison 01 Episode 07");
        Assert.AreEqual(MediaTypeModel.Episode, mediaType);
        Assert.AreEqual("Visitors", match.Groups[1].Value);
        Assert.AreEqual(1, int.Parse(match.Groups[2].Value));
        Assert.AreEqual(7, int.Parse(match.Groups[3].Value));
    }

    [TestCase(TestName = "GetMediaTypeAndInformations déduit l'épisode : Cas 3")]
    public void GetMediaTypeAndInformations_Episode_TestCase_3()
    {
        var mediaMatchingService = new MediaMatchingService();
        var (mediaType, match) = mediaMatchingService.GetMediaTypeAndInformations("Visitors Saison 1 Episode 7");
        Assert.AreEqual(MediaTypeModel.Episode, mediaType);
        Assert.AreEqual("Visitors", match.Groups[1].Value);
        Assert.AreEqual(1, int.Parse(match.Groups[2].Value));
        Assert.AreEqual(7, int.Parse(match.Groups[3].Value));
    }

    [TestCase(TestName = "GetMediaTypeAndInformations déduit l'épisode : Cas 4")]
    public void GetMediaTypeAndInformations_Episode_TestCase_4()
    {
        var mediaMatchingService = new MediaMatchingService();
        var (mediaType, match) = mediaMatchingService.GetMediaTypeAndInformations("[ Torrent911.net ] Visitors.S01E07.FRENCH.WEB.XviD-EXTREME.avi");
        Assert.AreEqual(MediaTypeModel.Episode, mediaType);
        Assert.AreEqual("Visitors", match.Groups[1].Value);
        Assert.AreEqual(1, int.Parse(match.Groups[2].Value));
        Assert.AreEqual(7, int.Parse(match.Groups[3].Value));
    }
}
// [ Torrent911.net ] Visitors.S01E02.FRENCH.WEB.XviD-EXTREME.avi