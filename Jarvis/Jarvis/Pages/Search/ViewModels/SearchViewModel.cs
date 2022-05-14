using Lib.Core;

namespace Jarvis;

public sealed class SearchViewModel
{
    public Func<bool, Task> IsLoadingChangedActionAsync;
    public Func<string, Task> SearchPatternChangedActionAsync;
    public Func<SearchResultViewModel, Task> SearchResultClickedActionAsync;

    private readonly ActionDebouncer _debouncer;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (value != _isLoading)
            {
                _isLoading = value;
                Task.Run(async () =>
                {
                    await IsLoadingChangedActionAsync(IsLoading);
                });
            }
        }
    }

    private string _searchPattern;
    public string SearchPattern
    {
        get => _searchPattern;
        set
        {
            if (value != _searchPattern)
            {
                _searchPattern = value;
                _debouncer.ExecuteAfterDelay(async () =>
                {
                    IsLoading = true;
                    await SearchPatternChangedActionAsync(SearchPattern);
                    IsLoading = false;
                });
            }
        }
    }

    private List<SearchResultViewModel> _searchResults = new();
    public List<SearchResultViewModel> SearchResults
    {
        get => _searchResults;
        set
        {
            if (value != _searchResults)
            {
                _searchResults = value;
                HasSearchResults = SearchResults != null && SearchResults.Any();
            }
        }
    }

    public bool HasSearchResults { get; private set; }

    public SearchViewModel()
    {
        _debouncer = new ActionDebouncer(1000);
    }

    public async Task OnDownloadSearchResultAsync(
        SearchResultViewModel searchResult)
    {
        IsLoading = true;
        await SearchResultClickedActionAsync(searchResult);
        IsLoading = false;
    }
}
