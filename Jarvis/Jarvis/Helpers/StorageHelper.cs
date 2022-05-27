namespace Jarvis;

public static class StorageHelper
{
    public static string GetTokensFile(
        string appdataDirectory)
    {
        return Path.Combine(appdataDirectory, "Tokens.json");
    }
}
