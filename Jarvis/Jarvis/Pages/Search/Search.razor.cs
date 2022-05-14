namespace Jarvis.Pages.Search;

public partial class Search : BlazorPageComponentBase
{
    public SearchViewModel SearchViewModel { get; set; }

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
            if (searchPattern.Length < 3)
            {
                SearchViewModel.SearchResults.Clear();
                return;
            }

            var torrent9TorrentScrapperService = new Torrent9TorrentScrapperService("https://www.torrent9.nl");
            var response = await torrent9TorrentScrapperService.GetStringAsync(searchPattern);
            var items = torrent9TorrentScrapperService.GetTorrentsFromHtml(response);
            SearchViewModel.SearchResults = items.Select(obj => new SearchResultViewModel(
                obj.Name,
                obj.Seeds,
                obj.Provider)
            ).OrderByDescending(obj => obj.Seeds).ToList();
        };
    }
}
