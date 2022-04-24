namespace Jarvis;

public interface IVPNClientBackgroundAgent
{
    VPNClientStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoop();

    Task StartClientAsync();

    Task StopClientAsync();
}
