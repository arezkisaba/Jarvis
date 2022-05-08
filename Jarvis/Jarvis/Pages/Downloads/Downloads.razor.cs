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
        DownloadsViewModel.IsLoadingChanged += async (sender, isLoading) => await UpdateUIAsync();
        DownloadsViewModel.SearchPatternChangedActionAsync = async (searchPattern) =>
        {
            await Task.Delay(5000);
        };
    }
}
