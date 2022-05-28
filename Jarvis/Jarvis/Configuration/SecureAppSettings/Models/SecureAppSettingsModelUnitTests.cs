using NUnit.Framework;

namespace Jarvis.Configuration.SecureAppSettings.Models;

[TestFixture]
public class SecureAppSettingsModelUnitTests
{
    [TestCase(TestName = "SecureAppSettingsModel s'initialise correctement")]
    public void SecureAppSettings_Construct1_TestCase()
    {
        var obj = new SecureAppSettingsModel(
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
