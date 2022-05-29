using Jarvis.Features.Agents.TorrentClientAgent.Models;
using Jarvis.Features.Services.TorrentClientService.Models;

namespace Jarvis.Features.Agents.TorrentClientAgent.Contracts;

public interface ITorrentClientAgent
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
