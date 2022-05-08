using Microsoft.AspNetCore.Components;

namespace Jarvis.Pages.Downloads;

public partial class Downloads : BlazorPageComponentBase
{
    [Inject]
    public IMediaCenterService MediaCenterService { get; set; }

    public DownloadsViewModel DownloadsViewModel { get; set; }

    protected override async Task OnInitializedAsync()
    {
        PageTitle = "Downloads";

        DownloadsViewModel = new DownloadsViewModel();
        DownloadsViewModel.IsLoadingChangedActionAsync = async (isLoading) =>
        {
            await UpdateUIAsync();
        };
        DownloadsViewModel.SearchPatternChangedActionAsync = async (searchPattern) =>
        {
            var torrent9TorrentScrapperService = new Torrent9TorrentScrapperService("https://www.torrent9.nl");
            var response = await torrent9TorrentScrapperService.GetStringAsync(searchPattern);
            var items = torrent9TorrentScrapperService.GetTorrentsFromHtml(response);
            await Task.Delay(5000);
        };
    }
}
