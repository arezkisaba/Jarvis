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

    public IIPResolverAgent IPResolverBackgroundAgent { get; }

    public IMediaStorageAgent MediaStorageBackgroundAgent { get; }

    public IGameControllerClientAgent GameControllerClientBackgroundAgent { get; }

    public IVPNClientAgent VPNClientBackgroundAgent { get; }

    public ITorrentClientAgent TorrentClientBackgroundAgent { get; }

    public JarvisService(
        ILogger<TransmissionAgent> logger,
        IIPResolverAgent ipResolverBackgroundAgent,
        IMediaStorageAgent mediaStorageBackgroundAgent,
        IGameControllerClientAgent gameControllerClientBackgroundAgent,
        IVPNClientAgent vpnClientBackgroundAgent,
        ITorrentClientAgent torrentClientBackgroundAgent)
    {
        _logger = logger;
        IPResolverBackgroundAgent = ipResolverBackgroundAgent;
        MediaStorageBackgroundAgent = mediaStorageBackgroundAgent;
        GameControllerClientBackgroundAgent = gameControllerClientBackgroundAgent;
        VPNClientBackgroundAgent = vpnClientBackgroundAgent;
        TorrentClientBackgroundAgent = torrentClientBackgroundAgent;
    }

    public async Task StartAsync()
    {
        IPResolverBackgroundAgent.StartBackgroundLoop();
        MediaStorageBackgroundAgent.StartBackgroundLoop();
        GameControllerClientBackgroundAgent.StartBackgroundLoopForControllerDetection();
        GameControllerClientBackgroundAgent.StartBackgroundLoopForCommands();
        GameControllerClientBackgroundAgent.StartBackgroundLoopForSoundAndMouseManagement();
        VPNClientBackgroundAgent.StartBackgroundLoop();
        TorrentClientBackgroundAgent.StartBackgroundLoop();

        _ = Task.Run(() => StartArcomShieldAsync());

        await VPNClientBackgroundAgent.StartClientAsync();
        await TorrentClientBackgroundAgent.StartClientAsync();
    }

    public async Task StopAsync()
    {
        await VPNClientBackgroundAgent.StopClientAsync();
        await TorrentClientBackgroundAgent.StopClientAsync();
    }

    #region Private use

    private async Task StartArcomShieldAsync()
    {
        while (true)
        {
            try
            {
                if (VPNClientBackgroundAgent.CurrentState == null || TorrentClientBackgroundAgent.CurrentState == null)
                {
                    continue;
                }

                if (VPNClientBackgroundAgent.CurrentState.IsActive && !TorrentClientBackgroundAgent.CurrentState.IsActive)
                {
                    await TorrentClientBackgroundAgent.StartClientAsync();
                }

                if (!VPNClientBackgroundAgent.CurrentState.IsActive && TorrentClientBackgroundAgent.CurrentState.IsActive)
                {
                    await TorrentClientBackgroundAgent.StopClientAsync();
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
