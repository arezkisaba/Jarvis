using Lib.Core;
using System.Diagnostics;

namespace Jarvis;

public class OpenVPNBackgroundAgent : IVPNClientBackgroundAgent
{
    private const int timeoutDelay = 30;

    private readonly AppSettings _appSettings;
    private readonly ILogger<OpenVPNBackgroundAgent> _logger;
    private readonly IProcessManager _processManager;
    private readonly INetworkManager _networkManager;
    private readonly ISecureAppSettingsService _secureAppSettingsService;
    private CancellationTokenSource _cancellationTokenSource;

    public VPNClientStateModel CurrentState { get; set; }

    public event EventHandler StateChanged;

    public OpenVPNBackgroundAgent(
        AppSettings appSettings,
        ILogger<OpenVPNBackgroundAgent> logger,
        IProcessManager processManager,
        INetworkManager networkManager,
        ISecureAppSettingsService secureAppSettingsService)
    {
        _appSettings = appSettings;
        _logger = logger;
        _processManager = processManager;
        _networkManager = networkManager;
        _secureAppSettingsService = secureAppSettingsService;
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

            var ovpnFiles = await DirectoryWrapper.GetFilesFromFolderAsync(_appSettings.vpnConfig.ovpnPath, includePath: true, filter: ".ovpn");
            var randomIndex = RandomHelper.GetInt(0, ovpnFiles.Count() - 1);

            var process = new Process();
            process.StartInfo.FileName = _appSettings.vpnConfig.executablePath;
            process.StartInfo.Arguments = string.Format(_appSettings.vpnConfig.executableParameters, ovpnFiles.ElementAt(randomIndex));
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += OnProcessOutputDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await Task.Delay(1000);
            var secureAppSettings = await _secureAppSettingsService.ReadAsync();
            process.StandardInput.WriteLine(secureAppSettings.OpenVPNUsername);
            await Task.Delay(1000);
            process.StandardInput.WriteLine(secureAppSettings.OpenVPNPassword);

            var counter = 0;
            while (!CurrentState.IsActive)
            {
                if (counter >= timeoutDelay)
                {
                    throw new JarvisException("Unable to start VPN client", new TimeoutException("VPN client still inactive"));
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

            var vpnProcesses = _processManager.GetByName(_appSettings.vpnConfig.executableName);
            if (vpnProcesses.Any())
            {
                foreach (var vpnProcess in vpnProcesses)
                {
                    _processManager.Kill(vpnProcess.Name);
                }
            }

            var counter = 0;
            while (CurrentState.IsActive)
            {
                if (counter >= timeoutDelay)
                {
                    throw new JarvisException("Unable to stop VPN client", new TimeoutException("VPN client still active"));
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
        VPNClientStateModel currentStateTemp;
        try
        {
            var hasVPNProcessActive = _processManager.GetByName(_appSettings.vpnConfig.executableName).Any();
            var hasVPNNetworkInterfaceActive = _networkManager.GetActiveInterfaces().Any(obj => obj.Name == _appSettings.vpnConfig.networkAdapterPattern);
            currentStateTemp = new VPNClientStateModel(
                title: "VPN client status",
                subtitle: _appSettings.vpnConfig.executableName,
                isActive: hasVPNProcessActive && hasVPNNetworkInterfaceActive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "XXXXX");
            currentStateTemp = new VPNClientStateModel(
                title: "VPN client status",
                subtitle: _appSettings.vpnConfig.executableName,
                isActive: false);
        }

        if (currentStateTemp.IsActive != CurrentState?.IsActive)
        {
            StateChanged?.Invoke(currentStateTemp, EventArgs.Empty);
        }

        CurrentState = currentStateTemp;
    }

    private void OnProcessOutputDataReceived(
        object sender,
        DataReceivedEventArgs e)
    {
        _logger.LogTrace($"{e.Data}");
    }

    #endregion
}
