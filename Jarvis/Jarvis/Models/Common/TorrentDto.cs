namespace Jarvis;

public class TorrentDto
{
    public string DescriptionUrl { get; set; }
    public string Name { get; set; }
    public string Provider { get; set; }
    public int Seeds { get; set; }
    public double Size { get; set; }
    public string Url { get; set; }

    public override string ToString()
    {
        return $"{Name} - {Url[..30]} - {Size} Bytes - SD : {Seeds} - {Provider}";
    }
}
