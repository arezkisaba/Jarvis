using Jarvis.Features.Agents.GameControllerAgent.Contracts;
using Jarvis.Features.Agents.IPResolverAgent.Contracts;
using Jarvis.Features.Agents.MediaStorageAgent.Contracts;
using Jarvis.Features.Agents.TorrentClientAgent;
using Jarvis.Features.Agents.TorrentClientAgent.Contracts;
using Jarvis.Features.Agents.VPNClientAgent.Contracts;

namespace Jarvis.Features.Services;

public class JarvisService
{
    private readonly ILogger<TransmissionAgent> _logger;

    public IIPResolverAgent IPResolverAgent { get; }

    public IMediaStorageAgent MediaStorageAgent { get; }

    public IGameControllerClientAgent GameControllerClientAgent { get; }

    public IVPNClientAgent VPNClientAgent { get; }

    public ITorrentClientAgent TorrentClientAgent { get; }

    public JarvisService(
        ILogger<TransmissionAgent> logger,
        IIPResolverAgent ipResolverAgent,
        IMediaStorageAgent mediaStorageAgent,
        IGameControllerClientAgent gameControllerClientAgent,
        IVPNClientAgent vpnClientAgent,
        ITorrentClientAgent torrentClientAgent)
    {
        _logger = logger;
        IPResolverAgent = ipResolverAgent;
        MediaStorageAgent = mediaStorageAgent;
        GameControllerClientAgent = gameControllerClientAgent;
        VPNClientAgent = vpnClientAgent;
        TorrentClientAgent = torrentClientAgent;
    }

    public async Task StartAsync()
    {
        IPResolverAgent.StartBackgroundLoop();
        MediaStorageAgent.StartBackgroundLoop();
        GameControllerClientAgent.StartBackgroundLoopForControllerDetection();
        GameControllerClientAgent.StartBackgroundLoopForCommands();
        GameControllerClientAgent.StartBackgroundLoopForSoundAndMouseManagement();
        VPNClientAgent.StartBackgroundLoop();
        TorrentClientAgent.StartBackgroundLoop();

        _ = Task.Run(() => StartArcomShieldAsync());

        await VPNClientAgent.StartClientAsync();
        await TorrentClientAgent.StartClientAsync();
    }

    public async Task StopAsync()
    {
        await VPNClientAgent.StopClientAsync();
        await TorrentClientAgent.StopClientAsync();
    }

    #region Private use

    private async Task StartArcomShieldAsync()
    {
        while (true)
        {
            try
            {
                if (VPNClientAgent.CurrentState == null || TorrentClientAgent.CurrentState == null)
                {
                    continue;
                }

                if (VPNClientAgent.CurrentState.IsActive && !TorrentClientAgent.CurrentState.IsActive)
                {
                    await TorrentClientAgent.StartClientAsync();
                }

                if (!VPNClientAgent.CurrentState.IsActive && TorrentClientAgent.CurrentState.IsActive)
                {
                    await TorrentClientAgent.StopClientAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "XXXXX");
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }

    #endregion
}
