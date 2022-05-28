using Jarvis.Helpers;

namespace Jarvis.Pages.Search.ViewModels;

public sealed class SearchResultViewModel
{
    public string Name { get; private set; }

    public string Size { get; private set; }

    public int Seeds { get; private set; }

    public string Provider { get; private set; }

    public string DescriptionUrl { get; private set; }

    public SearchResultViewModel(
        string name,
        double size,
        int seeds,
        string provider,
        string descriptionUrl)
    {
        Name = name;
        Size = FormatHelper.GetStringWithUnitFromSize(size);
        Seeds = seeds;
        Provider = provider;
        DescriptionUrl = descriptionUrl;
    }
}
