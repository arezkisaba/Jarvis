using Jarvis.Features.BackgroundAgents.VPNClientBackgroundAgent.Models;

namespace Jarvis.Features.BackgroundAgents.VPNClientBackgroundAgent.Contracts;

public interface IVPNClientBackgroundAgent
{
    VPNClientStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoop();

    Task StartClientAsync();

    Task StopClientAsync();

    void RefreshIsClientActive();
}
