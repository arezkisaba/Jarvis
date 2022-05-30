using Jarvis.Features.Agents.TorrentClientAgent.Contracts;
using Jarvis.Features.Services.MediaSizingService.Contracts;
using Jarvis.Features.Services.TorrentClientService.Contracts;
using Jarvis.Features.Services.TorrentScrapperService.Contracts;
using Jarvis.Pages.Search.ViewModels;
using Jarvis.Shared.Components.Toaster.Models;
using Jarvis.Shared.Components.Toaster.Services;
using Jarvis.Technical;
using Jarvis.Technical.Configuration.AppSettings.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Jarvis.Pages.Search;

public partial class Search : BlazorPageComponentBase
{
    [Inject]
    private IOptions<AppSettingsModel> AppSettings { get; set; }
    
    [Inject]
    private IStringLocalizer<App> AppLocalizer { get; set; }
    
    [Inject]
    private IStringLocalizer<Search> Localizer { get; set; }
    
    [Inject]
    private ITorrentClientService TorrentClientService { get; set; }
    
    [Inject]
    private ITorrentScrapperService TorrentScrapperService { get; set; }
    
    [Inject]
    private IMediaSizingService MediaSizingService { get; set; }
    
    [Inject]
    private ToasterService ToasterService { get; set; }

    private SearchViewModel SearchViewModel { get; set; }

    protected override void OnInitialized()
    {
        PageTitle = "Search";

        SearchViewModel = new SearchViewModel();
        SearchViewModel.IsLoadingChangedActionAsync = async (isLoading) =>
        {
            await UpdateUIAsync();
        };

        SearchViewModel.SearchPatternChangedActionAsync = async (searchPattern) =>
        {
            try
            {
                if (searchPattern.Length < 3)
                {
                    SearchViewModel.SearchResults = null;
                    return;
                }

                var items = await TorrentScrapperService.GetResultsAsync(searchPattern);
                SearchViewModel.SearchResults = items.Select(obj => new SearchResultViewModel(
                    mediaSizingService: MediaSizingService,
                    name: obj.Name,
                    size: obj.Size,
                    seeds: obj.Seeds,
                    provider: obj.Provider,
                    descriptionUrl: obj.DescriptionUrl)
                ).OrderByDescending(obj => obj.Seeds).ToList();
            }
            catch (Exception)
            {
                ToasterService.AddToast(Toast.CreateToast(AppLocalizer["Toaster.ErrorTitle"], AppLocalizer["Toaster.ErrorMessage"], ToastType.Danger, 2));
            }
        };
        SearchViewModel.SearchResultClickedActionAsync = async (searchResult) =>
        {
            try
            {
                var torrentLink = await TorrentScrapperService.GetDownloadLinkAsync(searchResult.Provider, searchResult.DescriptionUrl, null, null);
                var addDownloadTask = TorrentClientService.AddDownloadAsync(
                    searchResult.Name,
                    torrentLink,
                    AppSettings.Value.computer.downloadsFolder,
                    searchResult.Size,
                    searchResult.Seeds);
                var delayTask = Task.Delay(500);
                await Task.WhenAll(addDownloadTask, delayTask);

                ToasterService.AddToast(Toast.CreateToast(AppLocalizer["Toaster.InformationTitle"], Localizer["Toaster.DownloadAdded"], ToastType.Success, 2));
            }
            catch (Exception)
            {
                ToasterService.AddToast(Toast.CreateToast(AppLocalizer["Toaster.ErrorTitle"], AppLocalizer["Toaster.ErrorMessage"], ToastType.Danger, 2));
            }
        };
    }
}
