using Jarvis.Features.Agents.GameControllerAgent.Services.CECService.Contracts;
using Jarvis.Technical.Configuration.AppSettings.Models;
using System.Diagnostics;

namespace Jarvis.Features.Agents.GameControllerAgent.Services.CECService;

public class CECService : ICECService
{
    private readonly AppSettingsModel _appSettings;
    private readonly ILogger<CECService> _logger;

    public CECService(
        IOptions<AppSettingsModel> appSettings,
        ILogger<CECService> logger)
    {
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    public Task SwitchToComputerHDMISourceAsync(
        int hdmiSource)
    {
        return Task.Run(async () =>
        {
            var process = new Process();
            process.StartInfo.FileName = _appSettings.cecConfig.cecClientPath;
            process.StartInfo.Arguments = "-m";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += OnProcessOutputDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await Task.Delay(2000);
            process.StandardInput.WriteLine(GetPowerOnCommand());
            process.StandardInput.WriteLine(GetSwitchHDMISourceCommand(hdmiSource));
            await Task.Delay(5000);
            process.Kill();
        });
    }

    private void OnProcessOutputDataReceived(
        object sender,
        DataReceivedEventArgs e)
    {
        _logger.LogTrace($"{e.Data}");
    }

    private string GetPowerOnCommand()
    {
        return "tx 10:04";
    }

    private string GetPowerOffCommand()
    {
        return "tx 10:36";
    }

    private string GetSwitchHDMISourceCommand(
        int hdmiSource)
    {
        return $"tx 1F:82:{hdmiSource}0:00";
    }
}
