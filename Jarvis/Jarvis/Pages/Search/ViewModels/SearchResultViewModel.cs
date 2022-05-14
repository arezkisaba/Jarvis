using Lib.Core;

namespace Jarvis;

public sealed class SearchResultViewModel
{
    public string Name { get; private set; }

    public long Seeds { get; private set; }

    public string Provider { get; private set; }

    public SearchResultViewModel(
        string name,
        long seeds,
        string provider)
    {
        Name = name;
        Seeds = seeds;
        Provider = provider;
    }
}
