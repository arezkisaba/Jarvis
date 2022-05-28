using Jarvis.BackgroundAgents.VPNClientBackgroundAgent.Contracts;
using Jarvis.BackgroundAgents.VPNClientBackgroundAgent.Models;
using Jarvis.Configuration.AppSettings.Models;
using Jarvis.Configuration.SecureAppSettings.Models;
using Jarvis.Models.Exceptions;
using Lib.Core;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Jarvis.BackgroundAgents.VPNClientBackgroundAgent;

public class OpenVPNBackgroundAgent : IVPNClientBackgroundAgent
{
    private const int timeoutDelay = 30;

    private readonly AppSettingsModel _appSettings;
    private readonly SecureAppSettingsModel _secureAppSettings;
    private readonly ILogger<OpenVPNBackgroundAgent> _logger;
    private readonly IProcessManager _processManager;
    private readonly INetworkManager _networkManager;
    private CancellationTokenSource _cancellationTokenSource;

    public VPNClientStateModel CurrentState { get; set; }

    public event EventHandler StateChanged;

    public OpenVPNBackgroundAgent(
        IOptions<AppSettingsModel> appSettings,
        IOptions<SecureAppSettingsModel> secureAppSettings,
        ILogger<OpenVPNBackgroundAgent> logger,
        IProcessManager processManager,
        INetworkManager networkManager)
    {
        _appSettings = appSettings.Value;
        _secureAppSettings = secureAppSettings.Value;
        _logger = logger;
        _processManager = processManager;
        _networkManager = networkManager;
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
            var vpnProcesses = _processManager.GetByName(_appSettings.vpnConfig.executableName);
            vpnProcesses.ForEach(obj => _processManager.Kill(obj.Name));

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
            process.StandardInput.WriteLine(_secureAppSettings.OpenVPNUsername);
            await Task.Delay(1000);
            process.StandardInput.WriteLine(_secureAppSettings.OpenVPNPassword);

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
            vpnProcesses.ForEach(obj => _processManager.Kill(obj.Name));

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

    public void RefreshIsClientActive()
    {
        var resourceManager = new System.Resources.ResourceManager(
            "Jarvis.Resources.BackgroundAgents.VPNClientBackgroundAgent.OpenVPNBackgroundAgent",
            System.Reflection.Assembly.GetExecutingAssembly());

        VPNClientStateModel currentStateTemp;
        try
        {
            var hasVPNProcessActive = _processManager.GetByName(_appSettings.vpnConfig.executableName).Any();
            var hasVPNNetworkInterfaceActive = _networkManager.GetActiveInterfaces().Any(obj => obj.Name == _appSettings.vpnConfig.networkAdapterPattern);
            currentStateTemp = new VPNClientStateModel(
                title: resourceManager.GetString("Title"),
                subtitle: resourceManager.GetString("Subtitle"),
                isActive: hasVPNProcessActive && hasVPNNetworkInterfaceActive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "XXXXX");
            currentStateTemp = new VPNClientStateModel(
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

    private void OnProcessOutputDataReceived(
        object sender,
        DataReceivedEventArgs e)
    {
        _logger.LogTrace($"{e.Data}");
    }

    #endregion
}
