using Microsoft.AspNetCore.Components;

namespace Jarvis.Pages.Search;

public partial class Search : BlazorPageComponentBase
{
    private readonly Torrent9TorrentScrapperService _torrent9TorrentScrapperService;

    [Inject]
    public ITorrentDownloaderService TorrentDownloaderService { get; set; }

    [Inject]
    public AppSettings AppSettings { get; set; }

    public SearchViewModel SearchViewModel { get; set; }

    public Search()
    {
        _torrent9TorrentScrapperService = new Torrent9TorrentScrapperService("https://www.torrent9.nl");
    }

    protected override async Task OnInitializedAsync()
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

                var response = await _torrent9TorrentScrapperService.GetSearchResultsRawHtmlAsync(searchPattern);
                var items = _torrent9TorrentScrapperService.GetTorrentsFromHtml(response);
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
                var response = await _torrent9TorrentScrapperService.GetSearchResultDetailsRawHtmlAsync(searchResult.DescriptionUrl);
                var torrentLink = await _torrent9TorrentScrapperService.GetMagnetLinkFromHtmlAsync(response, null, null);
                var addDownloadTask = TorrentDownloaderService.AddDownloadAsync(torrentLink, AppSettings.computer.downloadsFolder);
                var waitTask = Task.Delay(500);
                await Task.WhenAll(addDownloadTask, waitTask);
            }
            catch (Exception)
            {
                throw;
            }
        };
    }
}
