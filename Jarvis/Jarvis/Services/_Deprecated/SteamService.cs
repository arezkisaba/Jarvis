using Lib.Core;
using System.Text;

namespace Jarvis;

public class SteamService
{
    private const string shortcutFile = "shortcuts.vdf";

    private readonly AppSettings _appSettings;
    private readonly ILogger<SteamService> _logger;
    private readonly string _targetShortcutFilePath;

    public SteamService(
        AppSettings appSettings,
        ILogger<SteamService> logger)
    {
        _appSettings = appSettings;
        _logger = logger;
        _targetShortcutFilePath = Path.Combine(_appSettings.steamUserDirectory, $"config\\{shortcutFile}");
    }

    public async Task OverwriteShortcutsAsync()
    {
        var cachedImageFiles = await DirectoryWrapper.GetFilesFromFolderAsync(_appSettings.appdataDirectory);
        var nonSteamGames = await GetNonSteamGamesAsync();

        var bytes = new List<byte>();
        WriteHeader(ref bytes);
        foreach (var nonSteamGame in nonSteamGames)
        {
            var httpClient = new HttpClient();

            var imagePath = string.Empty;
            foreach (var cachedImageFile in cachedImageFiles)
            {
                if (cachedImageFile.StartsWith(nonSteamGame.DisplayName))
                {
                    imagePath = Path.Combine(_appSettings.appdataDirectory, cachedImageFile);
                    break;
                }
            }

            ////if (string.IsNullOrWhiteSpace(imagePath))
            ////{
            ////    _logger.LogInformation($"Please enter thumbnail URL for {nonSteamGame.DisplayName} : ");
            ////    var imageUrl = Console.ReadLine();
            ////    var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
            ////    var extension = Path.GetExtension(imageUrl);
            ////    imagePath = Path.Combine(_appSettings.appdataDirectory, $"{nonSteamGame.DisplayName}{extension}");
            ////    File.WriteAllBytes(imagePath, imageBytes);
            ////    _logger.LogInformation($"{nonSteamGame.DisplayName} added to Steam");
            ////}

            WriteContentForGame(ref bytes, nonSteamGame, imagePath);
        }

        WriteFooter(ref bytes);

        File.WriteAllBytes(_targetShortcutFilePath, bytes.ToArray());
        _logger.LogInformation($"{shortcutFile} overwritted successfully");
    }

    public async Task<List<string>> GetSteamGameProcessesAsync()
    {
        var steamGameProcesses = new List<string>();
        var steamGamesDirectory = Path.Combine(_appSettings.steamDirectory, "steamapps\\common");
        var files = await DirectoryWrapper.GetFilesFromFolderAsync(steamGamesDirectory, recursive: true, includePath: false);
        foreach (var file in files)
        {
            if (file.EndsWith(".exe"))
            {
                steamGameProcesses.Add(file.Replace(".exe", ""));
            }
        }

        return steamGameProcesses;
    }

    private async Task<List<NonSteamGame>> GetNonSteamGamesAsync()
    {
        var nonSteamGames = new List<NonSteamGame>();
        nonSteamGames.AddRange(await GetGameboyGamesAsync());
        nonSteamGames.AddRange(await GetPlaystation01GamesAsync());
        nonSteamGames.AddRange(await GetPlaystation02GamesAsync());
        nonSteamGames.AddRange(await GetPlaystationPortableGamesAsync());
        return nonSteamGames;
    }

    private async Task<List<NonSteamGame>> GetGameboyGamesAsync()
    {
        var nonSteamGames = new List<NonSteamGame>();
        var authorizedFormats = new string[] { ".gba" };
        var gameFiles = await DirectoryWrapper.GetFilesFromFolderAsync(_appSettings.gameboyRomsDirectory, recursive: true, includePath: true);

        var i = 0;
        foreach (var gameFile in gameFiles)
        {
            var fileInfo = new FileInfo(gameFile);
            if (authorizedFormats.Any(obj => obj == fileInfo.Extension))
            {
                nonSteamGames.Add(new NonSteamGame(i, fileInfo.Name, _appSettings.gameboyEmulatorExecutable, string.Format(_appSettings.gameboyEmulatorExecutableArguments, $"\"{gameFile}\""), "Gameboy"));
                i++;
            }
        }

        return nonSteamGames;
    }

    private async Task<List<NonSteamGame>> GetPlaystation01GamesAsync()
    {
        var nonSteamGames = new List<NonSteamGame>();
        var authorizedFormats = new string[] { ".bin", ".img", ".iso" };
        var gameFiles = await DirectoryWrapper.GetFilesFromFolderAsync(_appSettings.playstation1RomsDirectory, recursive: true, includePath: true);

        var i = 0;
        foreach (var gameFile in gameFiles)
        {
            var fileInfo = new FileInfo(gameFile);
            if (authorizedFormats.Any(obj => obj == fileInfo.Extension))
            {
                nonSteamGames.Add(new NonSteamGame(i, fileInfo.Name, _appSettings.playstation1EmulatorExecutable, string.Format(_appSettings.playstation1EmulatorExecutableArguments, $"\"{gameFile}\""), "Playstation 01"));
                i++;
            }
        }

        return nonSteamGames;
    }

    private async Task<List<NonSteamGame>> GetPlaystation02GamesAsync()
    {
        var nonSteamGames = new List<NonSteamGame>();
        var authorizedFormats = new string[] { ".bin", ".img", ".iso" };
        var gameFiles = await DirectoryWrapper.GetFilesFromFolderAsync(_appSettings.playstation2RomsDirectory, recursive: true, includePath: true);

        var i = 0;
        foreach (var gameFile in gameFiles)
        {
            var fileInfo = new FileInfo(gameFile);
            if (authorizedFormats.Any(obj => obj == fileInfo.Extension))
            {
                nonSteamGames.Add(new NonSteamGame(i, fileInfo.Name, _appSettings.playstation2EmulatorExecutable, string.Format(_appSettings.playstation2EmulatorExecutableArguments, $"\"{gameFile}\""), "Playstation 02"));
                i++;
            }
        }

        return nonSteamGames;
    }

    private async Task<List<NonSteamGame>> GetPlaystationPortableGamesAsync()
    {
        var nonSteamGames = new List<NonSteamGame>();
        var authorizedFormats = new string[] { ".iso" };
        var gameFiles = await DirectoryWrapper.GetFilesFromFolderAsync(_appSettings.playstationPortableRomsDirectory, recursive: true, includePath: true);

        var i = 0;
        foreach (var gameFile in gameFiles)
        {
            var fileInfo = new FileInfo(gameFile);
            if (authorizedFormats.Any(obj => obj == fileInfo.Extension))
            {
                nonSteamGames.Add(new NonSteamGame(i, fileInfo.Name, _appSettings.playstationPortableEmulatorExecutable, string.Format(_appSettings.playstationPortableEmulatorExecutableArguments, $"\"{gameFile}\""), "Playstation Portable"));
                i++;
            }
        }

        return nonSteamGames;
    }

    private void WriteHeader(
        ref List<byte> bytes)
    {
        bytes.Add(0x00);
        bytes.AddRange(Encoding.UTF8.GetBytes("shortcuts"));
        bytes.Add(0x00);
    }

    private void WriteContentForGame(
        ref List<byte> bytes,
        NonSteamGame nonSteamGame,
        string imagePath)
    {
        bytes.Add(0x00);
        bytes.AddRange(Encoding.UTF8.GetBytes($"{nonSteamGame.Index}"));
        bytes.Add(0x00);
        bytes.Add(0x01);
        bytes.AddRange(Encoding.UTF8.GetBytes("AppName"));
        bytes.Add(0x00);
        bytes.AddRange(Encoding.UTF8.GetBytes(nonSteamGame.DisplayName));
        bytes.Add(0x00);
        bytes.Add(0x01);
        bytes.AddRange(Encoding.UTF8.GetBytes("Exe"));
        bytes.Add(0x00);
        bytes.AddRange(Encoding.UTF8.GetBytes($"\"{nonSteamGame.ExecutablePath}\""));
        bytes.Add(0x00);
        bytes.Add(0x01);
        bytes.AddRange(Encoding.UTF8.GetBytes("StartDir"));
        bytes.Add(0x00);
        bytes.AddRange(Encoding.UTF8.GetBytes($"\"{nonSteamGame.WorkingDirectory}\""));
        bytes.Add(0x00);
        bytes.Add(0x01);
        bytes.AddRange(Encoding.UTF8.GetBytes("icon"));
        bytes.Add(0x00);
        bytes.AddRange(Encoding.UTF8.GetBytes($"\"{imagePath}\""));
        bytes.Add(0x00);
        bytes.Add(0x01);
        bytes.AddRange(Encoding.UTF8.GetBytes("ShortcutPath"));
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x01);
        bytes.AddRange(Encoding.UTF8.GetBytes("LaunchOptions"));
        bytes.Add(0x00);

        if (!string.IsNullOrWhiteSpace(nonSteamGame.Arguments))
        {
            bytes.AddRange(Encoding.UTF8.GetBytes($"{nonSteamGame.Arguments}"));
        }

        bytes.Add(0x00);
        bytes.Add(0x02);
        bytes.AddRange(Encoding.UTF8.GetBytes("IsHidden"));
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x02);
        bytes.AddRange(Encoding.UTF8.GetBytes("AllowDesktopConfig"));
        bytes.Add(0x00);
        bytes.Add(0x01);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x02);
        bytes.AddRange(Encoding.UTF8.GetBytes("AllowOverlay"));
        bytes.Add(0x00);
        bytes.Add(0x01);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x02);
        bytes.AddRange(Encoding.UTF8.GetBytes("OpenVR"));
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x02);
        bytes.AddRange(Encoding.UTF8.GetBytes("Devkit"));
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x01);
        bytes.AddRange(Encoding.UTF8.GetBytes("DevkitGameID"));
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x02);
        bytes.AddRange(Encoding.UTF8.GetBytes("LastPlayTime"));
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.Add(0x00);
        bytes.AddRange(Encoding.UTF8.GetBytes("tags"));

        ////if (!string.IsNullOrWhiteSpace(nonSteamGame.Device))
        ////{
        ////    bytes.Add(0x00);
        ////    bytes.Add(0x01);
        ////    bytes.Add(0x30);
        ////    bytes.Add(0x00);
        ////    bytes.AddRange(Encoding.UTF8.GetBytes($"{nonSteamGame.Device}"));
        ////}

        bytes.Add(0x00);
        bytes.Add(0x08);
        bytes.Add(0x08);
    }

    private void WriteFooter(
        ref List<byte> bytes)
    {
        bytes.Add(0x08);
        bytes.Add(0x08);
    }
}
