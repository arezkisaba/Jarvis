namespace Jarvis;

public class TorrentDownloadModel
{
    public string Name { get; set; }

    public string Url { get; set; }

    public string DownloadDirectory { get; set; }

    public double PercentDone { get; set; }

    public string Size { get; set; }

    public int Seeds { get; set; }

    public string Provider { get; set; }

    public string Id { get; set; }

    public string HashString { get; set; }

    public TorrentDownloadModel(
        string name,
        string url,
        string downloadDirectory,
        double percentDone,
        string size,
        int seeds,
        string provider,
        string id,
        string hashString)
    {
        Name = name;
        Url = url;
        DownloadDirectory = downloadDirectory;
        PercentDone = percentDone;
        Size = size;
        Seeds = seeds;
        Provider = provider;
        Id = id;
        HashString = hashString;
    }
}
