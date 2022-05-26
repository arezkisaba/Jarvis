namespace Jarvis;

public interface ITorrentClientBackgroundAgent
{
    List<TorrentDownloadModel> TorrentDownloads { get; set; }

    TorrentClientStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    Action DownloadStateChangedAction { get; set; }

    void StartBackgroundLoop();

    Task StartClientAsync();

    Task StopClientAsync();

    Task AddDownloadAsync(
        string torrentUrl,
        string downloadDirectory,
        string size,
        int seeds);

    void RefreshIsClientActive();
}
