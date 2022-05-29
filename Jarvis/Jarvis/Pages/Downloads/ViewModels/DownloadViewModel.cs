using Jarvis.Features.Services.MediaSizingService.Contracts;

namespace Jarvis.Pages.Downloads.ViewModels;

public sealed class DownloadViewModel
{
    private readonly IMediaSizingService _mediaSizingService;

    public string Name { get; set; }

    public string Url { get; set; }

    public string DownloadDirectory { get; set; }

    public double PercentDone { get; set; }

    public long Size { get; set; }

    public string SizeDisplayed { get; private set; }

    public int Seeds { get; set; }

    public string Provider { get; set; }

    public string Id { get; set; }

    public string HashString { get; set; }

    public DownloadViewModel(
        IMediaSizingService mediaSizingService,
        string name,
        string url,
        string downloadDirectory,
        double percentDone,
        long size,
        int seeds,
        string provider,
        string id,
        string hashString)
    {
        _mediaSizingService = mediaSizingService;
        Name = name;
        Url = url;
        DownloadDirectory = downloadDirectory;
        PercentDone = percentDone;
        Size = size;
        SizeDisplayed = _mediaSizingService.ConvertBytesToStringWithUnit(size);
        Seeds = seeds;
        Provider = provider;
        Id = id;
        HashString = hashString;
    }
}
