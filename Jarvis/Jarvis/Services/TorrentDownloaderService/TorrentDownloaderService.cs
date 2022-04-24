using Lib.ApiServices.Transmission;
using Lib.Core;

namespace Jarvis;

public class TorrentDownloaderService : ITorrentDownloaderService
{
    private readonly ILogger<TorrentDownloaderService> _logger;
    private readonly ITransmissionApiService _transmissionApiService;
    private readonly IMediaService _mediaService;

    public List<GetModelResponse.DownloadsSectionItem> Downloads { get; set; }

    public TorrentDownloaderService(
        ILogger<TorrentDownloaderService> logger,
        ITransmissionApiService transmissionApiService,
        IMediaService mediaService)
    {
        _logger = logger;
        _transmissionApiService = transmissionApiService;
        _mediaService = mediaService;

        Downloads = new List<GetModelResponse.DownloadsSectionItem>();
    }

    public async Task StartDownloadsAsync()
    {
        try
        {
            _logger.LogInformation("Dowloading torrents...");

            Downloads = Downloads.OrderByDescending(obj => obj.seeders).ToList();
            foreach (var download in Downloads)
            {
                await AddDownloadAsync(download);
            }

            while (Downloads.Any(obj => obj.percentDone.GetValueOrDefault() < 100))
            {
                var downloadsFromTransmission = await _transmissionApiService.GetTorrentsAsync();
                foreach (var downloadFromTransmission in downloadsFromTransmission)
                {
                    var match = Downloads.FirstOrDefault(obj => obj.hashString == downloadFromTransmission.hashString);
                    if (match != null)
                    {
                        match.rateDownload = downloadFromTransmission.rateDownload;
                        match.rateUpload = downloadFromTransmission.rateUpload;
                        match.sizeWhenDone = downloadFromTransmission.sizeWhenDone;
                        match.leftUntilDone = downloadFromTransmission.leftUntilDone;
                        match.peersConnected = downloadFromTransmission.peersConnected;
                        match.peersGettingFromUs = downloadFromTransmission.peersGettingFromUs;
                        match.peersSendingToUs = downloadFromTransmission.peersSendingToUs;
                        match.percentDone = downloadFromTransmission.percentDone;
                        ////await _signalRHub.Clients.All.ReceiveDownloadUpdate(Serializer.JsonSerialize(match));
                    }
                }

                foreach (var torrentDownloadItem in Downloads)
                {
                    if (torrentDownloadItem.percentDone == 1 && !torrentDownloadItem.hasBeenCopied)
                    {
                        if (torrentDownloadItem.downloadMode == DownloadMode.Movie)
                        {
                            await _mediaService.RenameMovieOnDiskAsync(torrentDownloadItem.downloadDirectory, torrentDownloadItem.movieStorageTitle, torrentDownloadItem.movieYear);
                        }
                        else if (torrentDownloadItem.downloadMode == DownloadMode.Season)
                        {
                            for (var i = 1; i <= torrentDownloadItem.seasonEpisodesCount; i++)
                            {
                                await _mediaService.RenameEpisodeOnDiskAsync(torrentDownloadItem.downloadDirectory, torrentDownloadItem.tvShowStorageTitle, torrentDownloadItem.seasonNumber, i, torrentDownloadItem.language);
                            }
                        }
                        else if (torrentDownloadItem.downloadMode == DownloadMode.Episode)
                        {
                            await _mediaService.RenameEpisodeOnDiskAsync(torrentDownloadItem.downloadDirectory, torrentDownloadItem.tvShowStorageTitle, torrentDownloadItem.seasonNumber, torrentDownloadItem.episodeNumber, torrentDownloadItem.language);
                        }

                        Directory.Delete(torrentDownloadItem.downloadDirectory, recursive: true);
                        torrentDownloadItem.hasBeenCopied = true;
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            _logger.LogInformation("Torrents downloaded.");
        }
        catch (Exception ex)
        {
            throw new JarvisException("Failed to download torrents.", ex);
        }
    }

    public async Task AddDownloadAsync(
        GetModelResponse.DownloadsSectionItem download)
    {
        try
        {
            Directory.CreateDirectory(download.downloadDirectory);

            var torrendAdded = await _transmissionApiService.AddTorrentAsync(download.torrentUrl, download.downloadDirectory);
            if (torrendAdded?.arguments?.torrentadded != null)
            {
                download.id = torrendAdded.arguments.torrentadded.id;
                download.hashString = torrendAdded.arguments.torrentadded.hashString;
            }
            else if (torrendAdded?.arguments?.torrentduplicate != null)
            {
                download.id = torrendAdded.arguments.torrentduplicate.id;
                download.hashString = torrendAdded.arguments.torrentduplicate.hashString;
            }
            else
            {
                throw new Exception($"Failed to add torrent {download.GetDisplayTitle()}.");
            }

            ////await _signalRHub.Clients.All.ReceiveDownloadAdd(Serializer.JsonSerialize(download));

            _logger.LogInformation($"[{download.GetDisplayTitle()}] added to downloads.");
        }
        catch (Exception ex)
        {
            throw new JarvisException("Failed to add torrent.", ex);
        }
    }

    public async Task DeleteDownloadAsync(
        string hashstring)
    {
        try
        {
            var download = Downloads.Single(obj => obj.hashString == hashstring);
            Downloads.RemoveAll(obj => obj.hashString == download.hashString);
            await _transmissionApiService.DeleteTorrentAsync(download.id);
            ////await _signalRHub.Clients.All.ReceiveDownloadDelete(download.hashString);

            _logger.LogInformation($"[{download.GetDisplayTitle()}] deleted from downloads.");
        }
        catch (Exception ex)
        {
            throw new JarvisException("Failed to delete torrent from downloads.", ex);
        }
    }
}
