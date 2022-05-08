using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Jarvis.IntegrationTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public class MonIPServiceIntegrationTest
{
    [TestCase(TestName = "GetAsync renvois une adresse ip")]
    public async Task GetAsync_TestCase()
    {
        var ipResolverService = new MonIPService();
        var ip = await ipResolverService.GetAsync();
        Assert.IsTrue(!string.IsNullOrWhiteSpace(ip));
    }
}
