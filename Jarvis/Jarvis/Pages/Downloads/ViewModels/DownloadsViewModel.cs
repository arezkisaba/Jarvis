namespace Jarvis;

public sealed class DownloadsViewModel
{
    private IEnumerable<DownloadViewModel> _downloadItems;
    public IEnumerable<DownloadViewModel> DownloadItems
    {
        get => _downloadItems;
        set
        {
            if (value != _downloadItems)
            {
                _downloadItems = value;
                HasDownloadItems = DownloadItems != null && DownloadItems.Any();
            }
        }
    }

    public bool HasDownloadItems { get; private set; }

    public DownloadsViewModel(
        IEnumerable<DownloadViewModel> downloadItems)
    {
        DownloadItems = downloadItems;
    }
}
