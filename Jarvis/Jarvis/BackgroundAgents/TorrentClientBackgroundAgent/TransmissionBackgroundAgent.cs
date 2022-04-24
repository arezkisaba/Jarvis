using Lib.Core;

namespace Jarvis;

public class TransmissionBackgroundAgent : ITorrentClientBackgroundAgent
{
    private const int timeoutDelay = 30;

    private readonly AppSettings _appSettings;
    private readonly ILogger<TransmissionBackgroundAgent> _logger;
    private readonly IServiceManager _serviceManager;
    private readonly string _targetGlobalConfigFilePath;
    private CancellationTokenSource _cancellationTokenSource;

    public TorrentClientStateModel CurrentState { get; set; }

    public event EventHandler StateChanged;

    public TransmissionBackgroundAgent(
        AppSettings appSettings,
        ILogger<TransmissionBackgroundAgent> logger,
        IServiceManager serviceManager)
    {
        _appSettings = appSettings;
        _logger = logger;
        _serviceManager = serviceManager;
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
                    throw new JarvisException("Unable to start torrent client", new TimeoutException("Torrent client still inactive"));
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
                    throw new JarvisException("Unable to stop torrent client", new TimeoutException("Torrent client still active"));
                }

                await Task.Delay(500);
                counter++;
            }

            RefreshIsClientActive();
        });
    }

    #region Private use

    private void RefreshIsClientActive()
    {
        TorrentClientStateModel currentStateTemp;
        try
        {
            var windowsService = _serviceManager.GetAll().Single(obj => obj.Name == _appSettings.torrentConfig.serviceName);
            currentStateTemp = new TorrentClientStateModel(
                title: "Torrent client status",
                subtitle: _appSettings.torrentConfig.serviceName,
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
                title: "Torrent client status",
                subtitle: _appSettings.torrentConfig.serviceName,
                isActive: false);
        }

        if (currentStateTemp.IsActive != CurrentState?.IsActive)
        {
            StateChanged?.Invoke(currentStateTemp, EventArgs.Empty);
        }

        CurrentState = currentStateTemp;
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
