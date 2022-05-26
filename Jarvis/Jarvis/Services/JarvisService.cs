namespace Jarvis;

public class JarvisService
{
    private readonly ILogger<TransmissionBackgroundAgent> _logger;

    public IIPResolverBackgroundAgent IPResolverBackgroundAgent { get; }

    public IMediaStorageBackgroundAgent MediaStorageBackgroundAgent { get; }

    public IGameControllerClientBackgroundAgent GameControllerClientBackgroundAgent { get; }

    public IVPNClientBackgroundAgent VPNClientBackgroundAgent { get; }

    public ITorrentClientBackgroundAgent TorrentClientBackgroundAgent { get; }

    public JarvisService(
        ILogger<TransmissionBackgroundAgent> logger,
        IIPResolverBackgroundAgent ipResolverBackgroundAgent,
        IMediaStorageBackgroundAgent mediaStorageBackgroundAgent,
        IGameControllerClientBackgroundAgent gameControllerClientBackgroundAgent,
        IVPNClientBackgroundAgent vpnClientBackgroundAgent,
        ITorrentClientBackgroundAgent torrentClientBackgroundAgent)
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
