namespace Jarvis;

public static class StorageHelper
{
    public static string GetTokensFile(
        string appdataDirectory)
    {
        return Path.Combine(appdataDirectory, "Tokens.json");
    }

    public static string GetSecureAppSettingsFile(
        string appdataDirectory)
    {
        return Path.Combine(appdataDirectory, "Secrets.json");
    }
}
