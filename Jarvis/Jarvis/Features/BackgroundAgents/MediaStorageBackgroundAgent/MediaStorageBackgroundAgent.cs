using Jarvis.Features.BackgroundAgents.MediaStorageBackgroundAgent.Contracts;
using Jarvis.Features.BackgroundAgents.MediaStorageBackgroundAgent.Models;
using Jarvis.Features.BackgroundAgents.MediaStorageBackgroundAgent.Services.MediaStorageService.Contracts;
using Jarvis.Technical.Configuration.AppSettings.Models;

namespace Jarvis.Features.BackgroundAgents.MediaStorageBackgroundAgent;

public class MediaStorageBackgroundAgent : IMediaStorageBackgroundAgent
{
    private readonly AppSettingsModel _appSettings;
    private readonly IMediaStorageService _mediaStorageService;
    private CancellationTokenSource _cancellationTokenSource;

    public MediaStorageStateModel CurrentState { get; set; }

    public event EventHandler StateChanged;

    public MediaStorageBackgroundAgent(
        IOptions<AppSettingsModel> appSettings,
        IMediaStorageService mediaStorageService)
    {
        _appSettings = appSettings.Value;
        _mediaStorageService = mediaStorageService;
    }

    public void StartBackgroundLoop()
    {
        Task.Run(async () =>
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

            do
            {
                try
                {
                    var (UsedSpace, FreeSpace, TotalSpace) = _mediaStorageService.GetDriveInformations(_appSettings.computer.folderToScan);
                    var folders = _mediaStorageService.GetFolderContentAndSize(_appSettings.computer.folderToScan);
                    CurrentState = new MediaStorageStateModel(folders, UsedSpace, FreeSpace, TotalSpace);
                    StateChanged?.Invoke(CurrentState, EventArgs.Empty);
                }
                catch (Exception)
                {
                    // IGNORE
                }
            } while (await timer.WaitForNextTickAsync(_cancellationTokenSource.Token));
        });
    }
}
