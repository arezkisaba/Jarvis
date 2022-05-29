using Jarvis.Features.Agents.VPNClientAgent.Models;

namespace Jarvis.Features.Agents.VPNClientAgent.Contracts;

public interface IVPNClientAgent
{
    VPNClientStateModel CurrentState { get; set; }

    event EventHandler StateChanged;

    void StartBackgroundLoop();

    Task StartClientAsync();

    Task StopClientAsync();

    void RefreshIsClientActive();
}
