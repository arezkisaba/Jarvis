namespace Jarvis;

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
