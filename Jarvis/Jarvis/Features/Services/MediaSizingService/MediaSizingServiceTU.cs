using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Jarvis.Features.Services.MediaSizingService;

[TestFixture]
[ExcludeFromCodeCoverage]
public class MediaSizingServiceTU
{
    [TestCase(TestName = "ConvertBytesToStringWithUnit : Cas 1")]
    public void ConvertBytesToStringWithUnit_TestCase_1()
    {
        var mediaNamingService = new MediaSizingService();
        var strinWithUnit = mediaNamingService.ConvertBytesToStringWithUnit(1023);
        Assert.AreEqual("1023 B", strinWithUnit);
    }

    [TestCase(TestName = "ConvertBytesToStringWithUnit : Cas 2")]
    public void ConvertBytesToStringWithUnit_TestCase_2()
    {
        var mediaNamingService = new MediaSizingService();
        var strinWithUnit = mediaNamingService.ConvertBytesToStringWithUnit(1024);
        Assert.AreEqual("1 KB", strinWithUnit);
    }
}
