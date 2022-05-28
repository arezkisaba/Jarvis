using Jarvis.BackgroundAgents.TorrentClientBackgroundAgent.Contracts;
using Jarvis.Configuration.AppSettings.Models;
using Jarvis.Pages.Search.ViewModels;
using Jarvis.Services.TorrentScrapperService.Contracts;
using Jarvis.Shared.Components.Toaster.Models;
using Jarvis.Shared.Components.Toaster.Services;
using Jarvis.Technical;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Jarvis.Pages.Search;

public partial class Search : BlazorPageComponentBase
{
    [Inject]
    public IOptions<AppSettingsModel> AppSettings { get; set; }

    [Inject]
    public IStringLocalizer<App> AppLoc { get; set; }

    [Inject]
    public IStringLocalizer<Search> Loc { get; set; }

    [Inject]
    private ITorrentClientBackgroundAgent TorrentClientBackgroundAgent { get; set; }

    [Inject]
    private ITorrentScrapperService TorrentScrapperService { get; set; }

    [Inject]
    public ToasterService ToasterService { get; set; }

    public SearchViewModel SearchViewModel { get; set; }

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
                    obj.Name,
                    obj.Size,
                    obj.Seeds,
                    obj.Provider,
                    obj.DescriptionUrl)
                ).OrderByDescending(obj => obj.Seeds).ToList();
            }
            catch (Exception)
            {
                ToasterService.AddToast(Toast.CreateToast(AppLoc["Toaster.ErrorTitle"], AppLoc["Toaster.ErrorMessage"], ToastType.Danger, 2));
            }
        };
        SearchViewModel.SearchResultClickedActionAsync = async (searchResult) =>
        {
            try
            {
                var torrentLink = await TorrentScrapperService.GetDownloadLinkAsync(searchResult.Provider, searchResult.DescriptionUrl, null, null);
                var addDownloadTask = TorrentClientBackgroundAgent.AddDownloadAsync(
                    searchResult.Name,
                    torrentLink,
                    AppSettings.Value.computer.downloadsFolder,
                    searchResult.Size,
                    searchResult.Seeds);
                var delayTask = Task.Delay(500);
                await Task.WhenAll(addDownloadTask, delayTask);

                ToasterService.AddToast(Toast.CreateToast(AppLoc["Toaster.InformationTitle"], Loc["Toaster.DownloadAdded"], ToastType.Success, 2));
            }
            catch (Exception)
            {
                ToasterService.AddToast(Toast.CreateToast(AppLoc["Toaster.ErrorTitle"], AppLoc["Toaster.ErrorMessage"], ToastType.Danger, 2));
            }
        };
    }
}
