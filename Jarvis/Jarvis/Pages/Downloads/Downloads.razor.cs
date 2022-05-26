using Microsoft.AspNetCore.Components;

namespace Jarvis.Pages.Downloads;

public partial class Downloads : BlazorPageComponentBase
{
    [Inject]
    private ITorrentClientBackgroundAgent TorrentClientBackgroundAgent { get; set; }

    private DownloadsViewModel DownloadsViewModel { get; set; }

    protected override void OnInitialized()
    {
        PageTitle = "Downloads";

        TorrentClientBackgroundAgent.DownloadStateChangedAction = async () =>
        {
            UpdateDownloads();
            await UpdateUIAsync();
        };

        UpdateDownloads();
    }

    private void UpdateDownloads()
    {
        DownloadsViewModel = new DownloadsViewModel(
            TorrentClientBackgroundAgent.TorrentDownloads.Select(obj => new DownloadItemViewModel(
                name: obj.Name,
                url: obj.Url,
                downloadDirectory: obj.DownloadDirectory,
                percentDone: obj.PercentDone,
                size: obj.Size,
                seeds: obj.Seeds,
                provider: obj.Provider,
                id: obj.Id,
                hashString: obj.HashString)
            )
        );
    }
}
