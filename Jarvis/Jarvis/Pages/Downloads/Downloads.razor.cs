using Jarvis.Features.Agents.TorrentClientAgent.Contracts;
using Jarvis.Features.Services.MediaMatchingService.Contracts;
using Jarvis.Features.Services.MediaMatchingService.Models;
using Jarvis.Features.Services.MediaNamingService.Contracts;
using Jarvis.Features.Services.TorrentClientService.Contracts;
using Jarvis.Features.Services.TorrentClientService.Models;
using Jarvis.Pages.Downloads.ViewModels;
using Jarvis.Shared.Components.Toaster.Models;
using Jarvis.Shared.Components.Toaster.Services;
using Jarvis.Technical;
using Jarvis.Technical.Configuration.AppSettings.Models;
using Jarvis.Technical.Helpers;
using Lib.ApiServices.Tmdb;
using Lib.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Jarvis.Pages.Downloads;

public partial class Downloads : BlazorPageComponentBase
{
    [Inject]
    private IOptions<AppSettingsModel> AppSettings { get; set; }

    [Inject]
    public IStringLocalizer<App> AppLoc { get; set; }

    [Inject]
    public IStringLocalizer<Downloads> Loc { get; set; }

    [Inject]
    private ITorrentClientAgent TorrentClientBackgroundAgent { get; set; }

    [Inject]
    private ITorrentClientService TorrentClientService { get; set; }

    [Inject]
    private ITmdbApiService MediaDatabaseService { get; set; }

    [Inject]
    private IMediaNamingService MediaNamingService { get; set; }

    [Inject]
    private IMediaMatchingService MediaMatchingService { get; set; }

    [Inject]
    public ToasterService ToasterService { get; set; }

    private DownloadsViewModel DownloadsViewModel { get; set; }

    protected override void OnInitialized()
    {
        PageTitle = "Downloads";

        TorrentClientBackgroundAgent.DownloadStateChangedAction = async () =>
        {
            UpdateDownloads();
            await UpdateUIAsync();
        };

        TorrentClientBackgroundAgent.DownloadFinishedAction = async (download) =>
        {
            await OnVerraCeQuOnFaitIciAsync(download);
        };

        UpdateDownloads();
    }

    private void UpdateDownloads()
    {
        DownloadsViewModel = new DownloadsViewModel(
            TorrentClientService.TorrentDownloads.Select(obj => new DownloadViewModel(
                name: obj.Name,
                url: obj.Url,
                downloadDirectory: obj.DownloadDirectory,
                percentDone: obj.PercentDone,
                size: obj.Size,
                seeds: obj.Seeds,
                provider: obj.Provider,
                id: obj.Id,
                hashString: obj.HashString)
            )
        );
    }

    private async Task OnVerraCeQuOnFaitIciAsync(
        TorrentDownloadModel download)
    {
        try
        {
            var (mediaType, match) = MediaMatchingService.GetMediaTypeAndInformations(download.Name);
            if (mediaType == MediaTypeModel.Episode)
            {
                var torrentTitles = MediaNamingService.GetPossibleMovieTitles(match.Groups[1].Value);
                foreach (var torrentTitle in torrentTitles)
                {
                    var seasonNumber = int.Parse(match.Groups[2].Value);
                    var episodeNumber = int.Parse(match.Groups[3].Value);
                    var tvShows = await MediaDatabaseService.SearchTvShowAsync(torrentTitle);
                    if (tvShows.Any())
                    {
                        var tvShow = tvShows.ElementAt(0);
                        var seasons = await MediaDatabaseService.GetSeasonsAsync(tvShow.Id);
                        var season = seasons.SingleOrDefault(obj => obj.Number == seasonNumber);
                        if (season != null)
                        {
                            await RenameEpisodeOnDiskAsync(download.DownloadDirectory, tvShow.Title.TransformForStorage(), seasonNumber, episodeNumber, "FRENCH");
                            var message = string.Format(Loc["Toaster.DownloadEnded"], $"{MediaNamingService.GetDisplayNameForEpisode(tvShow.Title, seasonNumber, episodeNumber)}");
                            ToasterService.AddToast(Toast.CreateToast(AppLoc["Toaster.InformationTitle"], message, ToastType.Success, 2));
                            break;
                        }
                    }
                }
            }
            else if (mediaType == MediaTypeModel.Movie)
            {
                var torrentTitles = MediaNamingService.GetPossibleMovieTitles(match.Groups[1].Value);
                foreach (var torrentTitle in torrentTitles)
                {
                    var movies = await MediaDatabaseService.SearchMovieAsync(torrentTitle);
                    if (movies.Any())
                    {
                        var movie = movies.ElementAt(0);
                        await RenameMovieOnDiskAsync(download.DownloadDirectory, movie.Title.TransformForStorage(), movie.Year);
                        var message = string.Format(Loc["Toaster.DownloadEnded"], $"{MediaNamingService.GetDisplayNameForMovie(movie.Title)}");
                        ToasterService.AddToast(Toast.CreateToast(AppLoc["Toaster.InformationTitle"], message, ToastType.Success, 2));
                        break;
                    }
                }
            }

            await TorrentClientService.DeleteDownloadAsync(download.HashString);
        }
        catch (Exception)
        {
            ToasterService.AddToast(Toast.CreateToast(AppLoc["Toaster.ErrorTitle"], AppLoc["Toaster.ErrorMessage"], ToastType.Danger, 2));
        }
    }

    private async Task RenameMovieOnDiskAsync(
        string inputFolder,
        string movieStorageTitle,
        int? movieYear)
    {
        var files = await DirectoryWrapper.GetFilesFromFolderAsync(inputFolder, recursive: true, includePath: true);
        files = files.Where(file => AppSettings.Value.torrentConfig.videoExtensions.Contains(FormatHelper.GetExtensionFromNameOrPath(file))).OrderBy(obj => obj).ToList();

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

    private async Task RenameEpisodeOnDiskAsync(
        string inputFolder,
        string tvShowStorageTitle,
        int seasonNumber,
        int episodeNumber,
        string language)
    {
        var files = await DirectoryWrapper.GetFilesFromFolderAsync(inputFolder, recursive: true, includePath: true);
        files = files.Where(file => AppSettings.Value.torrentConfig.videoExtensions.Contains(FormatHelper.GetExtensionFromNameOrPath(file))).OrderBy(obj => obj).ToList();

        var shortEpisodeNames = FormatHelper.GetEpisodePossibleShortNames(seasonNumber, episodeNumber);
        var fileToRename = files.FirstOrDefault(file => shortEpisodeNames.Any(shortEpisodeName => file.ContainsIgnoreCase(shortEpisodeName)));
        if (fileToRename == null)
        {
            return;
        }

        var extension = FormatHelper.GetExtensionFromNameOrPath(fileToRename);
        var episodeVideoFile = GetEpisodeVideoFile(tvShowStorageTitle, seasonNumber, episodeNumber, language, extension);
        File.Move(fileToRename, episodeVideoFile);
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
        return Path.Combine(GetTvShowRootFolder(tvShowStorageTitle), FormatHelper.GetSeasonShortName(seasonNumber));
    }

    private string GetTvShowRootFolder(
        string tvShowStorageTitle)
    {
        return Path.Combine(GetTvShowBaseFolder(), tvShowStorageTitle);
    }

    private string GetMovieBaseFolder()
    {
        return AppSettings.Value.computer.moviesFolder;
    }

    private string GetTvShowBaseFolder()
    {
        return AppSettings.Value.computer.tvShowsFolder;
    }
}
