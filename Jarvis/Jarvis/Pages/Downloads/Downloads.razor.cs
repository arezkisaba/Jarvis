using Lib.ApiServices.Tmdb;
using Microsoft.AspNetCore.Components;

namespace Jarvis.Pages.Downloads;

public partial class Downloads : BlazorPageComponentBase
{
    [Inject]
    private ITorrentClientBackgroundAgent TorrentClientBackgroundAgent { get; set; }

    [Inject]
    private ITmdbApiService MediaDatabaseService { get; set; }

    [Inject]
    private IMediaMatchingService MediaMatchingService { get; set; }

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
            TorrentClientBackgroundAgent.TorrentDownloads.Select(obj => new DownloadViewModel(
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
                }
            }
        }

        //var movies = await MediaDatabaseService.GetMoviesInLibraryAsync();
    }
}
