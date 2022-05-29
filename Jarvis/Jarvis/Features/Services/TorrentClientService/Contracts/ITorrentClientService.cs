using Jarvis.Features.Services.TorrentClientService.Models;

namespace Jarvis.Features.Services.TorrentClientService.Contracts;

public interface ITorrentClientService
{
    List<TorrentDownloadModel> TorrentDownloads { get; set; }

    Task<IEnumerable<(string hashString, double percentDone)>> GetDownloadsAsync();

    Task AddDownloadAsync(
        string name,
        string torrentUrl,
        string downloadDirectory,
        long size,
        int seeds);

    Task DeleteDownloadAsync(
        string hashString);
}
