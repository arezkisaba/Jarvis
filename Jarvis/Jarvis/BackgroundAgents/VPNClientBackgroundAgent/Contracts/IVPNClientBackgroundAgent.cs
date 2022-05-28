using Jarvis.BackgroundAgents.VPNClientBackgroundAgent.Models;

namespace Jarvis.BackgroundAgents.VPNClientBackgroundAgent.Contracts;

public interface IVPNClientBackgroundAgent
{
    VPNClientStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoop();

    Task StartClientAsync();

    Task StopClientAsync();

    void RefreshIsClientActive();
}
