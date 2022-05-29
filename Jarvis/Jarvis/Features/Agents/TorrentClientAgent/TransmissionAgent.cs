using Jarvis.Features.Agents.TorrentClientAgent.Contracts;
using Jarvis.Features.Agents.TorrentClientAgent.Exceptions;
using Jarvis.Features.Agents.TorrentClientAgent.Models;
using Jarvis.Features.Services.TorrentClientService.Contracts;
using Jarvis.Features.Services.TorrentClientService.Models;
using Jarvis.Technical.Configuration.AppSettings.Models;
using Lib.Core;

namespace Jarvis.Features.Agents.TorrentClientAgent;

public class TransmissionAgent : ITorrentClientAgent
{
    private const int timeoutDelay = 30;

    private readonly AppSettingsModel _appSettings;
    private readonly ILogger<TransmissionAgent> _logger;
    private readonly IServiceManager _serviceManager;
    private readonly ITorrentClientService _torrentClientService;
    private readonly string _targetGlobalConfigFilePath;
    private CancellationTokenSource _cancellationTokenSource;

    public TorrentClientStateModel CurrentState { get; set; }

    public event EventHandler StateChanged;

    public Action DownloadStateChangedAction { get; set; }

    public Action<TorrentDownloadModel> DownloadFinishedAction { get; set; }

    public TransmissionAgent(
        IOptions<AppSettingsModel> appSettings,
        ILogger<TransmissionAgent> logger,
        IServiceManager serviceManager,
        ITorrentClientService torrentClientService)
    {
        _appSettings = appSettings.Value;
        _logger = logger;
        _serviceManager = serviceManager;
        _torrentClientService = torrentClientService;
        _targetGlobalConfigFilePath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), $"transmission-daemon\\settings.json");
    }

    public void StartBackgroundLoop()
    {
        Task.Run(async () =>
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

            do
            {
                await FillTorrentDownloadsAsync();
                RefreshIsClientActive();
            } while (await timer.WaitForNextTickAsync(_cancellationTokenSource.Token));
        });
    }

    public Task StartClientAsync()
    {
        return Task.Run(async () =>
        {
            RefreshIsClientActive();

            if (CurrentState.IsActive)
            {
                return;
            }

            await EnableConnectionsFromLocalNetworkAsync();
            _serviceManager.Start(_appSettings.torrentConfig.serviceName);

            var counter = 0;
            while (!CurrentState.IsActive)
            {
                if (counter >= timeoutDelay)
                {
                    throw new DemarrageServiceTorrentException("Unable to start torrent client", null);
                }

                await Task.Delay(500);
                counter++;
            }

            RefreshIsClientActive();
        });
    }

    public Task StopClientAsync()
    {
        return Task.Run(async () =>
        {
            RefreshIsClientActive();

            if (!CurrentState.IsActive)
            {
                return;
            }

            _serviceManager.Stop(_appSettings.torrentConfig.serviceName);

            var counter = 0;
            while (CurrentState.IsActive)
            {
                if (counter >= timeoutDelay)
                {
                    throw new ArretServiceTorrentException("Unable to stop torrent client", null);
                }

                await Task.Delay(500);
                counter++;
            }

            RefreshIsClientActive();
        });
    }

    #region Private use

    public void RefreshIsClientActive()
    {
        var resourceManager = new System.Resources.ResourceManager(
            "Jarvis.Resources.BackgroundAgents.TorrentClientBackgroundAgent.TransmissionBackgroundAgent",
            Assembly.GetExecutingAssembly());

        TorrentClientStateModel currentStateTemp;
        try
        {
            var windowsService = _serviceManager.GetAll().Single(obj => obj.Name == _appSettings.torrentConfig.serviceName);
            currentStateTemp = new TorrentClientStateModel(
                title: resourceManager.GetString("Title"),
                subtitle: resourceManager.GetString("Subtitle"),
                isActive: windowsService.IsStarted);

            if (windowsService.IsStartedOnBoot)
            {
                _serviceManager.DisableStartupOnBoot(_appSettings.torrentConfig.serviceName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "XXXXX");
            currentStateTemp = new TorrentClientStateModel(
                title: resourceManager.GetString("Title"),
                subtitle: resourceManager.GetString("Subtitle"),
                isActive: false);
        }

        if (currentStateTemp.IsActive != CurrentState?.IsActive)
        {
            StateChanged?.Invoke(currentStateTemp, EventArgs.Empty);
        }

        CurrentState = currentStateTemp;
    }

    private async Task FillTorrentDownloadsAsync()
    {
        try
        {
            var torrentDownloads = await _torrentClientService.GetDownloadsAsync();
            _torrentClientService.TorrentDownloads.ForEach(torrent =>
            {
                var (hashString, percentDone) = torrentDownloads.FirstOrDefault(obj => obj.hashString == torrent.HashString);
                if (hashString != null)
                {
                    if (torrent.PercentDone != percentDone)
                    {
                        torrent.PercentDone = percentDone;
                        if (torrent.PercentDone == 1)
                        {
                            DownloadFinishedAction?.Invoke(torrent);
                        }
                    }
                }
            });

            DownloadStateChangedAction?.Invoke();
        }
        catch (Exception)
        {
        }
    }

    private async Task EnableConnectionsFromLocalNetworkAsync()
    {
        var content = await File.ReadAllTextAsync(_targetGlobalConfigFilePath);
        content = content.Replace("\"rpc-whitelist\": \"127.0.0.1,::1\"", "\"rpc-whitelist\": \"127.0.0.1,::1,192.168.*.*\"");
        await File.WriteAllTextAsync(_targetGlobalConfigFilePath, content);
        _logger.LogInformation($"Configuration updated successfully");
    }

    #endregion
}
