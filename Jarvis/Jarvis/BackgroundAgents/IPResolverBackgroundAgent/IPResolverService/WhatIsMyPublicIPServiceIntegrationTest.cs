using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Jarvis.BackgroundAgents.IPResolverBackgroundAgent.IPResolverService;

[TestFixture]
[ExcludeFromCodeCoverage]
public class WhatIsMyPublicIPServiceIntegrationTest
{
    [TestCase(TestName = "GetAsync renvois une adresse ip")]
    public async Task GetAsync_TestCase()
    {
        var ipResolverService = new WhatIsMyPublicIPService();
        var ip = await ipResolverService.GetAsync();
        Assert.IsTrue(!string.IsNullOrWhiteSpace(ip));
    }
}
