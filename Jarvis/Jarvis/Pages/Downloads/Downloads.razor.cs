using Jarvis.Features.BackgroundAgents.TorrentClientBackgroundAgent.Contracts;
using Jarvis.Features.Services.MediaMatchingService.Contracts;
using Jarvis.Features.Services.MediaMatchingService.Models;
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
    private ITorrentClientBackgroundAgent TorrentClientBackgroundAgent { get; set; }

    [Inject]
    private ITorrentClientService TorrentClientService { get; set; }

    [Inject]
    private ITmdbApiService MediaDatabaseService { get; set; }

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
                var tvShowTitle = match.Groups[1].Value;
                var seasonNumber = int.Parse(match.Groups[2].Value);
                var episodeNumber = int.Parse(match.Groups[3].Value);
                var tvShows = await MediaDatabaseService.SearchTvShowAsync(tvShowTitle);
                if (tvShows.Any())
                {
                    var tvShow = tvShows.ElementAt(0);
                    var seasons = await MediaDatabaseService.GetSeasonsAsync(tvShow.Id);
                    var season = seasons.SingleOrDefault(obj => obj.Number == seasonNumber);
                    if (season != null)
                    {
                        var episodes = await MediaDatabaseService.GetEpisodesAsync(tvShow.Id, season.Number);
                        var episode = episodes.SingleOrDefault(obj => obj.Number == episodeNumber);

                        await RenameEpisodeOnDiskAsync(download.DownloadDirectory, tvShow.Title, seasonNumber, episodeNumber, "FRENCH");
                    }
                }
            }

            await TorrentClientService.DeleteDownloadAsync(download.HashString);

            ToasterService.AddToast(Toast.CreateToast(AppLoc["Toaster.InformationTitle"], Loc["Toaster.DownloadEnded"], ToastType.Success, 2));
        }
        catch (Exception)
        {
            ToasterService.AddToast(Toast.CreateToast(AppLoc["Toaster.ErrorTitle"], AppLoc["Toaster.ErrorMessage"], ToastType.Danger, 2));
        }
    }

    public async Task RenameEpisodeOnDiskAsync(
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

    private string GetTvShowBaseFolder()
    {
        return AppSettings.Value.computer.tvShowsFolder;
    }
}
