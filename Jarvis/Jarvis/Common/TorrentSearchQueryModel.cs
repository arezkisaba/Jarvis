namespace Jarvis;

public class TorrentSearchQueryModel
{
    public string Provider { get; set; }

    public string SearchTitle { get; set; }

    public string QueryString { get; set; }

    public List<TorrentDto> Results { get; set; }

    public TorrentSearchQueryModel(string provider, string searchTitle, string queryString, List<TorrentDto> results)
    {
        Provider = provider;
        SearchTitle = searchTitle;
        QueryString = queryString;
        Results = results;
    }

    public override string ToString()
    {
        return $"{Provider} / {SearchTitle} / {QueryString} / {Results.Count()}";
    }
}
