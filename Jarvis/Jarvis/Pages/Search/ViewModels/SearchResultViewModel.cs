using Lib.Core;

namespace Jarvis;

public sealed class SearchResultViewModel
{
    public string Name { get; private set; }

    public string Size { get; private set; }

    public long Seeds { get; private set; }

    public string Provider { get; private set; }

    public string DescriptionUrl { get; private set; }

    public SearchResultViewModel(
        string name,
        double size,
        long seeds,
        string provider,
        string descriptionUrl)
    {
        Name = name;
        Size = ConvertSizeToFriendlyDisplay(size);
        Seeds = seeds;
        Provider = provider;
        DescriptionUrl = descriptionUrl;
    }

    private static string ConvertSizeToFriendlyDisplay(
        double size)
    {
        return FormatHelper.GetStringWithUnitFromSize(size);
    }
}
