using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Jarvis.Pages.Search;

public partial class Search : BlazorPageComponentBase
{
    private readonly Torrent9TorrentScrapperService _torrent9TorrentScrapperService;

    [Inject]
    private NavigationManager NavManager { get; set; }

    [Inject]
    private ITorrentClientBackgroundAgent TorrentClientBackgroundAgent { get; set; }

    [Inject]
    private ITorrentScrapperService TorrentScrapperService { get; set; }

    [Inject]
    public IOptions<AppSettings> AppSettings { get; set; }

    public SearchViewModel SearchViewModel { get; set; }

    public bool ShowAlert { get; set; }

    public Search()
    {
        _torrent9TorrentScrapperService = new Torrent9TorrentScrapperService("https://www.torrent9.nl");
    }

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
                throw;
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
                ShowAlert = true;
            }
            catch (Exception)
            {
                throw;
            }
        };
    }
}
