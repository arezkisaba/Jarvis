using Lib.Core;
using System.Text.RegularExpressions;

namespace Jarvis;

public class JarvisService
{
    private readonly ILogger<TransmissionBackgroundAgent> _logger;

    public IIPResolverBackgroundAgent IPResolverBackgroundAgent { get; }

    public IMediaStorageBackgroundAgent MediaStorageBackgroundAgent { get; }

    public IGameControllerClientBackgroundAgent GameControllerClientBackgroundAgent { get; }

    public IVPNClientBackgroundAgent VPNClientBackgroundAgent { get; }

    public ITorrentClientBackgroundAgent TorrentClientBackgroundAgent { get; }

    public IMediaCenterService MediaCenterService { get; }

    public IMediaService MediaService { get; }

    public ITorrentSearchService TorrentSearchService { get; }

    public ITorrentDownloaderService DownloaderService { get; }

    public MediaStorageStateViewModel StatusViewModel { get; }

    public bool IsSearchingTorrent { get; set; }

    public JarvisService(
        ILogger<TransmissionBackgroundAgent> logger,
        IIPResolverBackgroundAgent ipResolverBackgroundAgent,
        IMediaStorageBackgroundAgent mediaStorageBackgroundAgent,
        IGameControllerClientBackgroundAgent gameControllerClientBackgroundAgent,
        IVPNClientBackgroundAgent vpnClientBackgroundAgent,
        ITorrentClientBackgroundAgent torrentClientBackgroundAgent,
        IMediaCenterService mediaCenterService,
        IMediaService mediaService,
        ITorrentSearchService torrentSearchService,
        ITorrentDownloaderService downloaderService)
    {
        _logger = logger;
        IPResolverBackgroundAgent = ipResolverBackgroundAgent;
        MediaStorageBackgroundAgent = mediaStorageBackgroundAgent;
        GameControllerClientBackgroundAgent = gameControllerClientBackgroundAgent;
        VPNClientBackgroundAgent = vpnClientBackgroundAgent;
        TorrentClientBackgroundAgent = torrentClientBackgroundAgent;
        MediaCenterService = mediaCenterService;
        MediaService = mediaService;
        TorrentSearchService = torrentSearchService;
        DownloaderService = downloaderService;
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

        await VPNClientBackgroundAgent.StartClientAsync();
        await TorrentClientBackgroundAgent.StartClientAsync();

        _ = Task.Run(() => StartArcomShieldAsync());
    }

    public async Task StopAsync()
    {
        await VPNClientBackgroundAgent.StopClientAsync();
        await TorrentClientBackgroundAgent.StopClientAsync();
    }

    ////public async Task StartSearchAsync()
    ////{
    ////    try
    ////    {
    ////        IsSearchingTorrent = true;

    ////        await MediaService.CreateDirectoriesIfDoesntExistsAsync();
    ////        var (movies, tvShows) = await MediaCenterService.FillMediasFromMediaCenterAndMediaDatabaseAsync();
    ////        await MediaCenterService.SynchronizeWatchStatusBetweenMediaCenterAndMediaDatabaseAsync();
    ////        await TorrentSearchService.SearchTorrentsAsync(movies, tvShows);
    ////        await DownloaderService.StartDownloadsAsync();
    ////    }
    ////    catch (JarvisException ex)
    ////    {
    ////        _logger.LogException(ex);
    ////    }
    ////    catch (Exception ex)
    ////    {
    ////        _logger.LogException(ex);
    ////    }
    ////    finally
    ////    {
    ////        IsSearchingTorrent = false;
    ////    }
    ////}

    #region Private use

    private async Task StartArcomShieldAsync()
    {
        while (true)
        {
            try
            {
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
