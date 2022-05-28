namespace Jarvis.Features.Services.TorrentClientService.Models;

public class TorrentDownloadModel
{
    public string Name { get; private set; }

    public string Url { get; private set; }

    public string DownloadDirectory { get; private set; }

    public double PercentDone { get; internal set; }

    public string Size { get; private set; }

    public int Seeds { get; private set; }

    public string Provider { get; private set; }

    public string Id { get; private set; }

    public string HashString { get; private set; }

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
