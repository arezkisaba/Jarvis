using Jarvis.Features.Services.TorrentScrapperService.Models;

namespace Jarvis.Features.Services.TorrentScrapperService.Contracts;

public interface ITorrentScrapperService
{
    Task<List<TorrentDto>> GetResultsAsync(
           string query);

    Task<string> GetDownloadLinkAsync(
        string provider,
        string descriptionUrl,
        string cookies,
        string userAgent);
}
