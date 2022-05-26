using Lib.Core;

namespace Jarvis;

public sealed class DownloadsViewModel
{
    private IEnumerable<DownloadItemViewModel> _downloadItems;
    public IEnumerable<DownloadItemViewModel> DownloadItems
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
        IEnumerable<DownloadItemViewModel> downloadItems)
    {
        DownloadItems = downloadItems;
    }
}
