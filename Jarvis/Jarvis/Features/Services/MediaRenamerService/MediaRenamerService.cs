using Jarvis.Features.Services.MediaMatchingService.Contracts;
using Jarvis.Features.Services.MediaNamingService.Contracts;
using Jarvis.Features.Services.MediaRenamerService.Contracts;
using Jarvis.Technical.Configuration.AppSettings.Models;
using Lib.Core;

namespace Jarvis.Features.Services.MediaRenamerService;

public class MediaRenamerService : IMediaRenamerService
{
    private readonly AppSettingsModel _appSettings;
    private readonly IMediaNamingService _mediaNamingService;
    private readonly IMediaMatchingService _mediaMatchingService;

    public MediaRenamerService(
        IOptions<AppSettingsModel> appSettings,
        IMediaNamingService mediaNamingService,
        IMediaMatchingService mediaMatchingService)
    {
        _appSettings = appSettings.Value;
        _mediaNamingService = mediaNamingService;
        _mediaMatchingService = mediaMatchingService;
    }

    public async Task RenameMovieAsync(
        string inputFolder,
        string titleForStorage,
        int? year)
    {
        var files = await DirectoryWrapper.GetFilesFromFolderAsync(inputFolder, recursive: true, includePath: true);
        files = files.Where(file => _appSettings.torrentConfig.videoExtensions.Contains(GetExtensionFromNameOrPath(file))).OrderBy(obj => obj).ToList();

        if (files.Count() == 1)
        {
            foreach (var file in files)
            {
                var extension = GetExtensionFromNameOrPath(file);
                File.Move(file, GetMovieVideoFile(titleForStorage, year, extension));
            }
        }
        else if (files.Count() > 1)
        {
            var i = 1;
            foreach (var file in files)
            {
                var extension = GetExtensionFromNameOrPath(file);
                File.Move(file, GetMovieVideoFile(titleForStorage, year, extension, i));
                i++;
            }
        }
    }

    public async Task RenameEpisodeAsync(
        string inputFolder,
        string titleForStorage,
        int seasonNumber,
        int episodeNumber,
        string language)
    {
        var files = await DirectoryWrapper.GetFilesFromFolderAsync(inputFolder, recursive: true, includePath: true);
        files = files.Where(file => _appSettings.torrentConfig.videoExtensions.Contains(GetExtensionFromNameOrPath(file))).OrderBy(obj => obj).ToList();

        var shortEpisodeNames = GetEpisodePossibleShortNames(seasonNumber, episodeNumber);
        var fileToRename = files.FirstOrDefault(file => shortEpisodeNames.Any(shortEpisodeName => file.ContainsIgnoreCase(shortEpisodeName)));
        if (fileToRename == null)
        {
            return;
        }

        var extension = GetExtensionFromNameOrPath(fileToRename);
        var episodeVideoFile = GetEpisodeVideoFile(titleForStorage, seasonNumber, episodeNumber, language, extension);

        if (!File.Exists(episodeVideoFile))
        {
            File.Move(fileToRename, episodeVideoFile);
        }
    }

    #region Private use

    private string GetMovieVideoFile(
        string storageTitle,
        int? year,
        string extension,
        int part = 0)
    {
        var partNumber = part == 0 ? string.Empty : $" - Part{part}";
        var fileName = $"{storageTitle} - {year}{partNumber}.{extension}";
        return Path.Combine(_appSettings.computer.moviesFolder, fileName.TransformForStorage());
    }

    private string GetEpisodeVideoFile(
        string tvShowStorageTitle,
        int seasonNumber,
        int episodeNumber,
        string language,
        string extension)
    {
        var seasonShortName = _mediaNamingService.GetShortNameForSeason(seasonNumber);
        var episodeShortName = _mediaNamingService.GetShortNameForEpisode(episodeNumber);
        var fileName = $"{tvShowStorageTitle} - {seasonShortName}{episodeShortName} - {language}.{extension}";
        var seasonFolder = GetSeasonRootFolder(tvShowStorageTitle, seasonNumber);
        if (!Directory.Exists(seasonFolder))
        {
            Directory.CreateDirectory(seasonFolder);
        }

        return Path.Combine(seasonFolder, fileName.TransformForStorage());
    }

    private string GetSeasonRootFolder(
        string tvShowStorageTitle,
        int seasonNumber)
    {
        return Path.Combine(_appSettings.computer.tvShowsFolder, tvShowStorageTitle, _mediaNamingService.GetShortNameForSeason(seasonNumber));
    }

    private IEnumerable<string> GetEpisodePossibleShortNames(
        int seasonNumber,
        int episodeNumber)
    {
        var episodeShortNames = new List<string>
        {
            seasonNumber + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "X" + episodeNumber,
            "X" + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "X " + episodeNumber,
            "X " + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "E" + episodeNumber,
            "E" + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "E " + episodeNumber,
            "E " + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "EP" + episodeNumber,
            "EP" + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "EP " + episodeNumber,
            "EP " + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "EPISODE" + episodeNumber,
            "EPISODE" + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "EPISODE " + episodeNumber,
            "EPISODE " + (episodeNumber < 10 ? "0" : "") + episodeNumber
        };
        return episodeShortNames;
    }

    private string GetExtensionFromNameOrPath(
        string nameOrPath)
    {
        if (!nameOrPath.Contains("."))
        {
            return string.Empty;
        }

        return nameOrPath[(nameOrPath.LastIndexOf(".") + 1)..];
    }

    #endregion
}
