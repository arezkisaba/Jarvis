using Jarvis.Technical.Configuration.SecureAppSettings.Models;
using NUnit.Framework;

namespace Jarvis.Pages.Configuration.ViewModels;

[TestFixture]
public class SecureAppSettingsViewModelTU
{
    [TestCase(TestName = "SecureAppSettingsViewModel s'initialise correctement")]
    public void SecureAppSettingsViewModel_Construct1_TestCase()
    {
        var obj = new SecureAppSettingsViewModel(new SecureAppSettingsModel(
            openVPNUsername: "openVPNUsername",
            openVPNPassword: "openVPNPassword",
            tmdbApiKey: "tmdbApiKey",
            tmdbAccessToken: "tmdbAccessToken",
            plexUsername: "plexUsername",
            plexPassword: "plexPassword"));
        Assert.AreEqual("openVPNUsername", obj.OpenVPNUsername);
        Assert.AreEqual("openVPNPassword", obj.OpenVPNPassword);
        Assert.AreEqual("tmdbApiKey", obj.TmdbApiKey);
        Assert.AreEqual("tmdbAccessToken", obj.TmdbAccessToken);
        Assert.AreEqual("plexUsername", obj.PlexUsername);
        Assert.AreEqual("plexPassword", obj.PlexPassword);
    }
}
