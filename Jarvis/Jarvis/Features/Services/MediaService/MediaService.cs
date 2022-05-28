using Jarvis.Features.Services.MediaService.Contracts;
using Jarvis.Technical.Configuration.AppSettings.Models;
using Jarvis.Technical.Helpers;
using Jarvis.Technical.Models.Domain;
using Lib.Core;
using System.Text.RegularExpressions;

namespace Jarvis.Features.Services.MediaService;

public class MediaService : IMediaService
{
    private const string EpisodeExpression = @"^(.*?) - S([0-9]{2,})E([0-9]{2,}) - (.*?)\.(.*?)$";

    private readonly AppSettingsModel _appSettings;
    private readonly string _downloadsFolder;
    private readonly string _moviesFolder;
    private readonly string _tvShowsFolder;

    public MediaService(
        IOptions<AppSettingsModel> appSettings)
    {
        _appSettings = appSettings.Value;
        _downloadsFolder = _appSettings.computer.downloadsFolder;
        _moviesFolder = _appSettings.computer.moviesFolder;
        _tvShowsFolder = _appSettings.computer.tvShowsFolder;
    }

    public void CreateDirectoriesIfDoesntExists()
    {
        Directory.CreateDirectory(GetDownloadFolder());
        Directory.CreateDirectory(GetMovieBaseFolder());
        Directory.CreateDirectory(GetTvShowBaseFolder());
    }

    public async Task CreateTvShowFoldersAsync(
        TvShow tvShow)
    {
        Directory.CreateDirectory(GetTvShowRootFolder(tvShow.StorageTitle));

        foreach (var season in tvShow.Seasons)
        {
            Directory.CreateDirectory(GetSeasonRootFolder(tvShow.StorageTitle, season.Number));
        }
    }

    public async Task<IEnumerable<Movie>> GetMissingMoviesOnDiskAsync(
        IEnumerable<Movie> movies)
    {
        var missingMovies = new List<Movie>();
        var files = await DirectoryWrapper.GetFilesFromFolderAsync(GetMovieBaseFolder(), recursive: true, includePath: false);

        foreach (var movie in movies)
        {
            var isMovieMissing = !files.Any(obj => obj.ContainsIgnoreCase($"{movie.StorageTitle} - {movie.Year}"));
            if (isMovieMissing)
            {
                missingMovies.Add(movie);
            }
        }

        return missingMovies;
    }

    public async Task<IEnumerable<Episode>> GetMissingEpisodesForSeasonOnDiskAsync(
        Season season,
        string language)
    {
        var missingEpisodes = new List<Episode>();
        var files = await DirectoryWrapper.GetFilesFromFolderAsync(GetSeasonRootFolder(season.TvShow.StorageTitle, season.Number), recursive: true, includePath: false);
        foreach (var episode in season.Episodes)
        {
            var isEpisodeExists = false;
            foreach (var shortName in FormatHelper.GetEpisodePossibleShortNames(season.Number, episode.Number))
            {
                isEpisodeExists = files.Any(obj =>
                {
                    var match = GetFirstMatch(obj, EpisodeExpression);
                    if (match == null)
                    {
                        return false;
                    }

                    ////var _all = match.Groups[0];
                    ////var _title = match.Groups[1];
                    ////var _season = match.Groups[2];
                    var _episode = match.Groups[3];
                    var _language = match.Groups[4];
                    ////var _extension = match.Groups[5];

                    return $"E{_episode.Value}" == shortName && _language.Value == language;
                });

                if (isEpisodeExists)
                {
                    break;
                }
            }

            if (!isEpisodeExists)
            {
                missingEpisodes.Add(episode);
            }
        }

        return missingEpisodes;
    }

    public async Task RenameMovieOnDiskAsync(
        string baseFolder,
        string movieStorageTitle,
        int? movieYear)
    {
        var files = await DirectoryWrapper.GetFilesFromFolderAsync(baseFolder, recursive: true, includePath: true);
        files = files.Where(file => _appSettings.torrentConfig.videoExtensions.Contains(FormatHelper.GetExtensionFromNameOrPath(file))).OrderBy(obj => obj).ToList();

        if (files.Count() == 1)
        {
            foreach (var file in files)
            {
                var extension = FormatHelper.GetExtensionFromNameOrPath(file);
                File.Move(file, GetMovieVideoFile(movieStorageTitle, movieYear, extension));
            }
        }
        else if (files.Count() > 1)
        {
            var i = 1;
            foreach (var file in files)
            {
                var extension = FormatHelper.GetExtensionFromNameOrPath(file);
                File.Move(file, GetMovieVideoFile(movieStorageTitle, movieYear, extension, i));
                i++;
            }
        }
    }

    public async Task RenameEpisodeOnDiskAsync(
        string baseFolder,
        string tvShowStorageTitle,
        int seasonNumber,
        int episodeNumber,
        string language)
    {
        var files = await DirectoryWrapper.GetFilesFromFolderAsync(baseFolder, recursive: true, includePath: true);
        files = files.Where(file => _appSettings.torrentConfig.videoExtensions.Contains(FormatHelper.GetExtensionFromNameOrPath(file))).OrderBy(obj => obj).ToList();

        var shortEpisodeNames = FormatHelper.GetEpisodePossibleShortNames(seasonNumber, episodeNumber);
        var fileToRename = files.FirstOrDefault(file => shortEpisodeNames.Any(shortEpisodeName => file.ContainsIgnoreCase(shortEpisodeName)));
        if (fileToRename == null)
        {
            return;
        }

        var extension = FormatHelper.GetExtensionFromNameOrPath(fileToRename);
        File.Move(fileToRename, GetEpisodeVideoFile(tvShowStorageTitle, seasonNumber, episodeNumber, language, extension));
    }

    public string GetDownloadFolder()
    {
        return _downloadsFolder;
    }

    #region Private use

    private string GetMovieBaseFolder()
    {
        return _moviesFolder;
    }

    private string GetTvShowBaseFolder()
    {
        return _tvShowsFolder;
    }

    private string GetTvShowRootFolder(
        string tvShowStorageTitle)
    {
        return Path.Combine(GetTvShowBaseFolder(), tvShowStorageTitle);
    }

    private string GetSeasonRootFolder(
        string tvShowStorageTitle,
        int seasonNumber)
    {
        return Path.Combine(GetTvShowRootFolder(tvShowStorageTitle), FormatHelper.GetSeasonShortName(seasonNumber));
    }

    private string GetMovieVideoFile(
        string storageTitle,
        int? year,
        string extension,
        int part = 0)
    {
        var partNumber = part == 0 ? string.Empty : $" - Part{part}";
        var fileName = $"{storageTitle} - {year}{partNumber}.{extension}";
        return Path.Combine(GetMovieBaseFolder(), fileName.TransformForStorage());
    }

    private string GetEpisodeVideoFile(
        string tvShowStorageTitle,
        int seasonNumber,
        int episodeNumber,
        string language,
        string extension)
    {
        var seasonShortName = FormatHelper.GetSeasonShortName(seasonNumber);
        var episodeShortName = FormatHelper.GetEpisodeShortName(episodeNumber);
        var fileName = $"{tvShowStorageTitle} - {seasonShortName}{episodeShortName} - {language}.{extension}";
        return Path.Combine(GetSeasonRootFolder(tvShowStorageTitle, seasonNumber), fileName.TransformForStorage());
    }

    private Match GetFirstMatch(
        string target,
        string pattern)
    {
        var regex = new Regex(pattern);
        var matches = regex.Matches(target);
        if (matches.Count > 0)
        {
            var match = matches[0];
            if (match.Groups.Count > 0)
            {
                return match;
            }
        }

        return null;
    }

    #endregion
}
