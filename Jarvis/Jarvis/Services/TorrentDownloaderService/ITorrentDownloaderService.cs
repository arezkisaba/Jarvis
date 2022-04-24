namespace Jarvis;

public interface ITorrentDownloaderService
{
    List<GetModelResponse.DownloadsSectionItem> Downloads { get; set; }

    Task AddDownloadAsync(
        GetModelResponse.DownloadsSectionItem download);

    Task DeleteDownloadAsync(
        string hashstring);

    Task StartDownloadsAsync();
}
