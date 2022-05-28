namespace Jarvis.Configuration.AppSettings.Models;

public class AppSettingsModel
{
    public string appName { get; set; }

    public string[] gameProcesses { get; set; }

    public string appdataDirectory { get; set; }

    public string saveDirectory { get; set; }

    public string babylonDirectory { get; set; }

    public string steamDirectory { get; set; }

    public string steamUserDirectory { get; set; }

    public string emulationDirectory { get; set; }

    public string gameboyRomsDirectory { get; set; }

    public string gameboyEmulatorExecutable { get; set; }

    public string gameboyEmulatorExecutableArguments { get; set; }

    public string playstation1RomsDirectory { get; set; }

    public string playstation1EmulatorExecutable { get; set; }

    public string playstation1EmulatorExecutableArguments { get; set; }

    public string playstation2RomsDirectory { get; set; }

    public string playstation2EmulatorExecutable { get; set; }

    public string playstation2EmulatorExecutableArguments { get; set; }

    public ComputerSection computer { get; set; }

    public CECConfigSection cecConfig { get; set; }

    public ServiceConfigSection serviceConfig { get; set; }

    public TorrentConfigSection torrentConfig { get; set; }

    public VPNConfigSection vpnConfig { get; set; }

    public class ComputerSection
    {
        public string folderToScan { get; set; }
        public string downloadsFolder { get; set; }
        public string moviesFolder { get; set; }
        public string tvShowsFolder { get; set; }
    }

    public class CECConfigSection
    {
        public string cecClientPath { get; set; }
        public int computerHDMISource { get; set; }
        public int defaultHDMISource { get; set; }
    }

    public class ServiceConfigSection
    {
        public PlexConfigSection plexConfig { get; set; }
        public TransmissionConfigSection transmissionConfig { get; set; }
    }

    public class PlexConfigSection
    {
        public string url { get; set; }
    }

    public class TransmissionConfigSection
    {
        public string url { get; set; }
    }

    public class TorrentConfigSection
    {
        public string serviceName { get; set; }
        public string webProxyPath { get; set; }
        public string language { get; set; }
        public string[] excludedPatterns { get; set; }
        public MediaConfigSection mediaConfig { get; set; }
        public int minSeeders { get; set; }
        public string providers { get; set; }
        public string[] videoExtensions { get; set; }
    }

    public class MediaConfigSection
    {
        public int movieMinSize { get; set; }
        public int movieRecentMinSize { get; set; }
        public long movieMaxSize { get; set; }
        public int tvShowMinSize { get; set; }
        public long tvShowMaxSize { get; set; }
        public int seasonMinSize { get; set; }
        public long seasonMaxSize { get; set; }
        public int episodeMinSize { get; set; }
        public int episodeMaxSize { get; set; }
    }

    public class VPNConfigSection
    {
        public string executableName { get; set; }
        public string executablePath { get; set; }
        public string executableParameters { get; set; }
        public string ovpnPath { get; set; }
        public string networkAdapterPattern { get; set; }
    }
}
