namespace Jarvis;

public class NonSteamGame
{
    public int Index { get; set; }

    public string DisplayName { get; set; }

    public string ExecutablePath { get; set; }

    public string Arguments { get; set; }

    public string Device { get; set; }

    public string WorkingDirectory { get; set; }

    public NonSteamGame(
        int index,
        string displayName,
        string executablePath,
        string arguments,
        string device)
    {
        Index = index;
        DisplayName = displayName;
        ExecutablePath = executablePath;
        Arguments = arguments;
        Device = device;
        WorkingDirectory = new FileInfo(ExecutablePath).Directory.FullName;
    }
}
