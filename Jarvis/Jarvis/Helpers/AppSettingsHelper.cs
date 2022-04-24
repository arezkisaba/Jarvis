namespace Jarvis;

public static class AppSettingsHelper
{
    public static void FillAppSettings(
        AppSettings appSettings)
    {
        appSettings.appdataDirectory = string.Format(appSettings.appdataDirectory, Environment.GetEnvironmentVariable("AppData"));
        appSettings.steamDirectory = string.Format(appSettings.steamDirectory, appSettings.babylonDirectory);
        appSettings.steamUserDirectory = string.Format(appSettings.steamUserDirectory, appSettings.steamDirectory);
        appSettings.emulationDirectory = string.Format(appSettings.emulationDirectory, appSettings.babylonDirectory);
        appSettings.gameboyRomsDirectory = string.Format(appSettings.gameboyRomsDirectory, appSettings.emulationDirectory);
        appSettings.gameboyEmulatorExecutable = string.Format(appSettings.gameboyEmulatorExecutable, appSettings.emulationDirectory);
        appSettings.playstation1RomsDirectory = string.Format(appSettings.playstation1RomsDirectory, appSettings.emulationDirectory);
        appSettings.playstation1EmulatorExecutable = string.Format(appSettings.playstation1EmulatorExecutable, appSettings.emulationDirectory);
        appSettings.playstation1EmulatorExecutableArguments = string.Format(appSettings.playstation1EmulatorExecutableArguments, appSettings.emulationDirectory, "{0}");
        appSettings.playstation2RomsDirectory = string.Format(appSettings.playstation2RomsDirectory, appSettings.emulationDirectory);
        appSettings.playstation2EmulatorExecutable = string.Format(appSettings.playstation2EmulatorExecutable, appSettings.emulationDirectory);
        appSettings.playstationPortableRomsDirectory = string.Format(appSettings.playstationPortableRomsDirectory, appSettings.emulationDirectory);
        appSettings.playstationPortableEmulatorExecutable = string.Format(appSettings.playstationPortableEmulatorExecutable, appSettings.emulationDirectory);
        appSettings.computer.folderToScan = string.Format(appSettings.computer.folderToScan, appSettings.babylonDirectory);
        appSettings.computer.downloadsFolder = string.Format(appSettings.computer.downloadsFolder, appSettings.babylonDirectory);
        appSettings.computer.moviesFolder = string.Format(appSettings.computer.moviesFolder, appSettings.babylonDirectory);
        appSettings.computer.tvShowsFolder = string.Format(appSettings.computer.tvShowsFolder, appSettings.babylonDirectory);
        appSettings.vpnConfig.ovpnPath = string.Format(appSettings.vpnConfig.ovpnPath, appSettings.babylonDirectory);
    }
}
