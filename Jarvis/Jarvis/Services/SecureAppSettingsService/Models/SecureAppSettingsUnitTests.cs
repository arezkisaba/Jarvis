using NUnit.Framework;

namespace Jarvis.UnitTests;

[TestFixture]
public class SecureAppSettingsUnitTests
{
    [TestCase(TestName = "SecureAppSettings s'initialise correctement")]
    public void SecureAppSettings_Construct1_TestCase()
    {
        var obj = new SecureAppSettings(
            openVPNUsername: "openVPNUsername",
            openVPNPassword: "openVPNPassword",
            tmdbApiKey: "tmdbApiKey",
            tmdbAccessToken: "tmdbAccessToken",
            plexUsername: "plexUsername",
            plexPassword: "plexPassword");
        Assert.AreEqual("openVPNUsername", obj.OpenVPNUsername);
        Assert.AreEqual("openVPNPassword", obj.OpenVPNPassword);
        Assert.AreEqual("tmdbApiKey", obj.TmdbApiKey);
        Assert.AreEqual("tmdbAccessToken", obj.TmdbAccessToken);
        Assert.AreEqual("plexUsername", obj.PlexUsername);
        Assert.AreEqual("plexPassword", obj.PlexPassword);
    }
}
