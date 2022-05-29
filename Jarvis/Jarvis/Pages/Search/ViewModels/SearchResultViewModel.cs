using Jarvis.Features.Services.MediaSizingService.Contracts;

namespace Jarvis.Pages.Search.ViewModels;

public sealed class SearchResultViewModel
{
    private readonly IMediaSizingService _mediaSizingService;

    public string Name { get; private set; }

    public long Size { get; private set; }

    public string SizeDisplayed { get; }

    public int Seeds { get; private set; }

    public string Provider { get; private set; }

    public string DescriptionUrl { get; private set; }

    public SearchResultViewModel(
        IMediaSizingService mediaSizingService,
        string name,
        long size,
        int seeds,
        string provider,
        string descriptionUrl)
    {
        _mediaSizingService = mediaSizingService;
        Name = name;
        Size = size;
        SizeDisplayed = _mediaSizingService.ConvertBytesToStringWithUnit(size);
        Seeds = seeds;
        Provider = provider;
        DescriptionUrl = descriptionUrl;
    }
}
