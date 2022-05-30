using Jarvis.Features.Agents.TorrentClientAgent.Contracts;
using Jarvis.Features.Services.MediaMatchingService.Contracts;
using Jarvis.Features.Services.MediaMatchingService.Models;
using Jarvis.Features.Services.MediaNamingService.Contracts;
using Jarvis.Features.Services.MediaRenamerService.Contracts;
using Jarvis.Features.Services.MediaSizingService.Contracts;
using Jarvis.Features.Services.TorrentClientService.Contracts;
using Jarvis.Features.Services.TorrentClientService.Models;
using Jarvis.Pages.Downloads.ViewModels;
using Jarvis.Shared.Components.Toaster.Models;
using Jarvis.Shared.Components.Toaster.Services;
using Jarvis.Technical;
using Jarvis.Technical.Configuration.AppSettings.Models;
using Lib.ApiServices.Tmdb;
using Lib.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Jarvis.Pages.Downloads;

public partial class Downloads : BlazorPageComponentBase
{
    [Inject]
    private IStringLocalizer<Downloads> Localizer { get; set; }
    
    [Inject] 
    private ITorrentClientAgent TorrentClientAgent { get; set; }
    
    [Inject]
    private ITorrentClientService TorrentClientService { get; set; }
    
    [Inject]
    private IMediaSizingService MediaSizingService { get; set; }

    private DownloadsViewModel DownloadsViewModel { get; set; }

    protected override void OnInitialized()
    {
        PageTitle = "Downloads";

        TorrentClientAgent.DownloadStateChangedAction = async () =>
        {
            UpdateDownloads();
            await UpdateUIAsync();
        };

        UpdateDownloads();
    }

    private void UpdateDownloads()
    {
        DownloadsViewModel = new DownloadsViewModel(
            TorrentClientService.TorrentDownloads.Select(obj => new DownloadViewModel(
                mediaSizingService: MediaSizingService,
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
