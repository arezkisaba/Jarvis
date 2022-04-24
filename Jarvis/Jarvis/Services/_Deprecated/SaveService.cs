using Lib.Core;

namespace Jarvis;

public class SaveService
{
    private readonly AppSettings _appSettings;
    private readonly ILogger<SaveService> _logger;

    public SaveService(
        AppSettings appSettings,
        ILogger<SaveService> logger)
    {
        _appSettings = appSettings;
        _logger = logger;
    }

    public async Task ArchiveAsync()
    {
        await ArchiveGameBoySavesAsync();
        await ArchivePlaystation01SavesAsync();
        await ArchivePlaystation02SavesAsync();
    }

    private async Task ArchiveGameBoySavesAsync()
    {
        var files = await DirectoryWrapper.GetFilesFromFolderAsync(_appSettings.emulationDirectory, recursive: true, includePath: true, filter: ".sav");
        foreach (var file in files)
        {
            var fileName = new FileInfo(file).Name;
            var gameName = new FileInfo(file).Directory.Name;
            var targetFolder = Path.Combine(_appSettings.saveDirectory, $"Game Boy\\{gameName}");
            Directory.CreateDirectory(targetFolder);
            File.Copy(file, Path.Combine(targetFolder, fileName));
            _logger.LogInformation($"{fileName} saved successfully");
        }
    }

    private async Task ArchivePlaystation01SavesAsync()
    {
        var files = await DirectoryWrapper.GetFilesFromFolderAsync(_appSettings.emulationDirectory, recursive: true, includePath: true, filter: ".mcr");
        foreach (var file in files)
        {
            var fileName = new FileInfo(file).Name;
            var targetFolder = Path.Combine(_appSettings.saveDirectory, $"Playstation - 01");
            Directory.CreateDirectory(targetFolder);
            File.Copy(file, Path.Combine(targetFolder, fileName));
            _logger.LogInformation($"{fileName} saved successfully");
        }
    }

    private async Task ArchivePlaystation02SavesAsync()
    {
        var files = await DirectoryWrapper.GetFilesFromFolderAsync(_appSettings.emulationDirectory, recursive: true, includePath: true, filter: ".ps2");
        foreach (var file in files)
        {
            var fileName = new FileInfo(file).Name;
            var targetFolder = Path.Combine(_appSettings.saveDirectory, $"Playstation - 02");
            Directory.CreateDirectory(targetFolder);
            File.Copy(file, Path.Combine(targetFolder, fileName));
            _logger.LogInformation($"{fileName} saved successfully");
        }
    }
}
