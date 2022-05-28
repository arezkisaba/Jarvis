using Jarvis.Features.BackgroundAgents.TorrentClientBackgroundAgent.Models;
using Jarvis.Features.Services.TorrentClientService.Models;

namespace Jarvis.Features.BackgroundAgents.TorrentClientBackgroundAgent.Contracts;

public interface ITorrentClientBackgroundAgent
{
    TorrentClientStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    Action DownloadStateChangedAction { get; set; }

    Action<TorrentDownloadModel> DownloadFinishedAction { get; set; }

    void StartBackgroundLoop();

    Task StartClientAsync();

    Task StopClientAsync();

    void RefreshIsClientActive();
}
