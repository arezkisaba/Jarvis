using Lib.ApiServices.Transmission;
using Lib.Core;

namespace Jarvis;

public class TransmissionBackgroundAgent : ITorrentClientBackgroundAgent
{
    private const int timeoutDelay = 30;

    private readonly AppSettings _appSettings;
    private readonly ILogger<TransmissionBackgroundAgent> _logger;
    private readonly IServiceManager _serviceManager;
    private readonly ITransmissionApiService _transmissionApiService;
    private readonly string _targetGlobalConfigFilePath;
    private CancellationTokenSource _cancellationTokenSource;

    public List<TorrentDownloadModel> TorrentDownloads { get; set; } = new();

    public TorrentClientStateModel CurrentState { get; set; }

    public event EventHandler StateChanged;

    public Action DownloadStateChangedAction { get; set; }

    public TransmissionBackgroundAgent(
        AppSettings appSettings,
        ILogger<TransmissionBackgroundAgent> logger,
        IServiceManager serviceManager,
        ITransmissionApiService transmissionApiService)
    {
        _appSettings = appSettings;
        _logger = logger;
        _serviceManager = serviceManager;
        _transmissionApiService = transmissionApiService;
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

    public async Task AddDownloadAsync(
        string url,
        string downloadDirectory,
        string size,
        int seeds)
    {
        try
        {
            Directory.CreateDirectory(downloadDirectory);

            var torrendAdded = await _transmissionApiService.AddTorrentAsync(url, downloadDirectory);
            if (torrendAdded?.arguments?.torrentadded != null)
            {
                var id = torrendAdded.arguments.torrentadded.id;
                var hashString = torrendAdded.arguments.torrentadded.hashString;
                TorrentDownloads.Add(new TorrentDownloadModel(
                    name: torrendAdded.arguments.torrentadded.name,
                    url: url,
                    downloadDirectory: downloadDirectory,
                    percentDone: 0,
                    size: size,
                    seeds: seeds,
                    provider: "Torrent9",
                    id: id.ToString(),
                    hashString: hashString)
                );
            }
            else if (torrendAdded?.arguments?.torrentduplicate != null)
            {
                ////var id = torrendAdded.arguments.torrentduplicate.id;
                ////var hashString = torrendAdded.arguments.torrentduplicate.hashString;
                throw new Exception($"Failed to add torrent.");
            }
            else
            {
                throw new Exception($"Failed to add torrent.");
            }

            _logger.LogInformation($"Torrent added to downloads.");
        }
        catch (Exception ex)
        {
            throw new JarvisException("Failed to add torrent.", ex);
        }
    }

    public async Task DeleteDownloadAsync(
        string hashString)
    {
        try
        {
            var download = TorrentDownloads.Single(obj => obj.HashString == hashString);
            await _transmissionApiService.DeleteTorrentAsync(int.Parse(download.Id));
            TorrentDownloads.RemoveAll(obj => obj.HashString == download.HashString);
        }
        catch (Exception ex)
        {
            throw new JarvisException("Failed to delete torrent from downloads.", ex);
        }
    }

    #region Private use

    public void RefreshIsClientActive()
    {
        var resourceManager = new System.Resources.ResourceManager(
            "Jarvis.Resources.BackgroundAgents.TorrentClientBackgroundAgent.TransmissionBackgroundAgent",
            System.Reflection.Assembly.GetExecutingAssembly());

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
            var torrentDownloads = await _transmissionApiService.GetTorrentsAsync();
            TorrentDownloads.ForEach(torrent =>
            {
                var match = torrentDownloads.FirstOrDefault(obj => obj.hashString == torrent.HashString);
                if (match != null)
                {
                    torrent.Name = match.name;
                    torrent.PercentDone = match.percentDone;
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
