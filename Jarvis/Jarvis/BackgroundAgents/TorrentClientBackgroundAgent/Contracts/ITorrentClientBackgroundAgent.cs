using Jarvis.BackgroundAgents.TorrentClientBackgroundAgent.Models;

namespace Jarvis.BackgroundAgents.TorrentClientBackgroundAgent.Contracts;

public interface ITorrentClientBackgroundAgent
{
    List<TorrentDownloadModel> TorrentDownloads { get; set; }

    TorrentClientStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    Action DownloadStateChangedAction { get; set; }

    Action<TorrentDownloadModel> DownloadFinishedAction { get; set; }

    void StartBackgroundLoop();

    Task StartClientAsync();

    Task StopClientAsync();

    Task AddDownloadAsync(
        string name,
        string torrentUrl,
        string downloadDirectory,
        string size,
        int seeds);

    Task DeleteDownloadAsync(
        string hashString);

    void RefreshIsClientActive();
}
