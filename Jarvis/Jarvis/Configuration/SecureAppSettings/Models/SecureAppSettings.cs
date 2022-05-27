namespace Jarvis;

public class SecureAppSettings
{
    public string OpenVPNUsername { get; set; }

    public string OpenVPNPassword { get; set; }

    public string TmdbApiKey { get; set; }

    public string TmdbAccessToken { get; set; }

    public string TmdbSessionId { get; set; }

    public string PlexUsername { get; set; }

    public string PlexPassword { get; set; }

    public SecureAppSettings()
    {
    }

    public SecureAppSettings(
        string openVPNUsername,
        string openVPNPassword,
        string tmdbApiKey,
        string tmdbAccessToken,
        string plexUsername,
        string plexPassword)
    {
        OpenVPNUsername = openVPNUsername;
        OpenVPNPassword = openVPNPassword;
        TmdbApiKey = tmdbApiKey;
        TmdbAccessToken = tmdbAccessToken;
        PlexUsername = plexUsername;
        PlexPassword = plexPassword;
    }

    public SecureAppSettings(
        string openVPNUsername,
        string openVPNPassword,
        string tmdbApiKey,
        string tmdbAccessToken,
        string tmdbSessionId,
        string plexUsername,
        string plexPassword)
    {
        OpenVPNUsername = openVPNUsername;
        OpenVPNPassword = openVPNPassword;
        TmdbApiKey = tmdbApiKey;
        TmdbAccessToken = tmdbAccessToken;
        TmdbSessionId = tmdbSessionId;
        PlexUsername = plexUsername;
        PlexPassword = plexPassword;
    }
}
