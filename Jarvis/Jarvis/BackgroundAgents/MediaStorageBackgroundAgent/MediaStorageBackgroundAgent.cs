namespace Jarvis;

public class MediaStorageBackgroundAgent : IMediaStorageBackgroundAgent
{
    private readonly IMediaStorageService _mediaStorageService;
    private readonly AppSettings _appSettings;
    private CancellationTokenSource _cancellationTokenSource;

    public MediaStorageStateModel CurrentState { get; set; }

    public event EventHandler StateChanged;

    public MediaStorageBackgroundAgent(
        IMediaStorageService mediaStorageService,
        AppSettings appSettings)
    {
        _mediaStorageService = mediaStorageService;
        _appSettings = appSettings;
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
