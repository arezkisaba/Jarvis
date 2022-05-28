using Jarvis.Technical.Configuration.SecureAppSettings.Models;
using System.ComponentModel.DataAnnotations;

namespace Jarvis.Pages.Configuration.ViewModels;

public sealed class SecureAppSettingsViewModel
{
    [Required(ErrorMessage = "The OpenVPN username is required")]
    public string OpenVPNUsername { get; set; }

    [Required(ErrorMessage = "The OpenVPN password is required")]
    public string OpenVPNPassword { get; set; }

    [Required(ErrorMessage = "The TMDB api key is required")]
    public string TmdbApiKey { get; set; }

    [Required(ErrorMessage = "The TMDB access token is required")]
    public string TmdbAccessToken { get; set; }

    [Required(ErrorMessage = "The Plex username is required")]
    public string PlexUsername { get; set; }

    [Required(ErrorMessage = "The Plex password is required")]
    public string PlexPassword { get; set; }

    public SecureAppSettingsViewModel()
    {
    }

    public SecureAppSettingsViewModel(
        SecureAppSettingsModel model)
    {
        OpenVPNUsername = model.OpenVPNUsername;
        OpenVPNPassword = model.OpenVPNPassword;
        TmdbApiKey = model.TmdbApiKey;
        TmdbAccessToken = model.TmdbAccessToken;
        PlexUsername = model.PlexUsername;
        PlexPassword = model.PlexPassword;
    }

    public SecureAppSettingsModel ToDomain()
    {
        return new SecureAppSettingsModel(
            OpenVPNUsername,
            OpenVPNPassword,
            TmdbApiKey,
            TmdbAccessToken,
            PlexUsername,
            PlexPassword);
    }
}
