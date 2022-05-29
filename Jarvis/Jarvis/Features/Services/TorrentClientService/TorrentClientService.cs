using Jarvis.Features.Services.TorrentClientService.Contracts;
using Jarvis.Features.Services.TorrentClientService.Exceptions;
using Jarvis.Features.Services.TorrentClientService.Models;
using Lib.ApiServices.Transmission;

namespace Jarvis.Features.Services.TorrentClientService;

public class TorrentClientService : ITorrentClientService
{
    private readonly ILogger<TorrentClientService> _logger;
    private readonly ITransmissionApiService _transmissionApiService;

    public List<TorrentDownloadModel> TorrentDownloads { get; set; } = new();

    public TorrentClientService(
        ILogger<TorrentClientService> logger,
        ITransmissionApiService transmissionApiService)
    {
        _logger = logger;
        _transmissionApiService = transmissionApiService;
    }

    public async Task<IEnumerable<(string hashString, double percentDone)>> GetDownloadsAsync()
    {
        var torrentDownloads = await _transmissionApiService.GetTorrentsAsync();
        return torrentDownloads.Select(obj => (obj.hashString, obj.percentDone)).ToList();
    }

    public async Task AddDownloadAsync(
        string name,
        string url,
        string downloadDirectory,
        string size,
        int seeds)
    {
        try
        {
            Directory.CreateDirectory(downloadDirectory);

            var torrendAdded = await _transmissionApiService.AddTorrentAsync(url, downloadDirectory);
            if (torrendAdded?.arguments?.torrentadded != null)
            {
                var id = torrendAdded.arguments.torrentadded.id;
                var hashString = torrendAdded.arguments.torrentadded.hashString;
                TorrentDownloads.Add(new TorrentDownloadModel(
                    name: name,
                    url: url,
                    downloadDirectory: downloadDirectory,
                    size: size,
                    seeds: seeds,
                    provider: "Torrent9",
                    id: id.ToString(),
                    hashString: hashString)
                );
            }
            else if (torrendAdded?.arguments?.torrentduplicate != null)
            {
                ////var id = torrendAdded.arguments.torrentduplicate.id;
                ////var hashString = torrendAdded.arguments.torrentduplicate.hashString;
            }
            else
            {
                throw new Exception($"Failed to add torrent.");
            }

            _logger.LogInformation($"Torrent added to downloads.");
        }
        catch (Exception ex)
        {
            throw new Exceptions.TorrentAddException("Failed to add torrent.", ex);
        }
    }

    public async Task DeleteDownloadAsync(
        string hashString)
    {
        try
        {
            var download = TorrentDownloads.Single(obj => obj.HashString == hashString);
            await _transmissionApiService.DeleteTorrentAsync(int.Parse(download.Id));
            TorrentDownloads.RemoveAll(obj => obj.HashString == download.HashString);
        }
        catch (Exception ex)
        {
            throw new TorrentDeleteException("Failed to delete torrent from downloads.", ex);
        }
    }
}
