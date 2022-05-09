using Lib.Core;

namespace Jarvis;

public sealed class DownloadsViewModel
{
    public Func<bool, Task> IsLoadingChangedActionAsync;
    public Func<string, Task> SearchPatternChangedActionAsync;

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

    public DownloadsViewModel()
    {
        _debouncer = new ActionDebouncer(2000);
    }
}