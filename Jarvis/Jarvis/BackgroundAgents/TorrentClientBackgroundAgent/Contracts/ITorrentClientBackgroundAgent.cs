namespace Jarvis;

public interface ITorrentClientBackgroundAgent
{
    TorrentClientStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoop();

    Task StartClientAsync();

    Task StopClientAsync();
}
