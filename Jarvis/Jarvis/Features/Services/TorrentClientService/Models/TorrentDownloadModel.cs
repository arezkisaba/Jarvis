namespace Jarvis.Features.Services.TorrentClientService.Models;

public class TorrentDownloadModel
{
    public string Name { get; }

    public string Url { get; }

    public string DownloadDirectory { get; }

    public double PercentDone { get; internal set; }

    public string Size { get; }

    public int Seeds { get; }

    public string Provider { get; }

    public string Id { get; }

    public string HashString { get; }

    public TorrentDownloadModel(
        string name,
        string url,
        string downloadDirectory,
        string size,
        int seeds,
        string provider,
        string id,
        string hashString)
    {
        Name = name;
        Url = url;
        DownloadDirectory = downloadDirectory;
        PercentDone = 0;
        Size = size;
        Seeds = seeds;
        Provider = provider;
        Id = id;
        HashString = hashString;
    }
}
