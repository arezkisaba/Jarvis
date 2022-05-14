using Lib.Core;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Jarvis;

public abstract class TorrentScrapperServiceBase
{
    public abstract bool IsActive { get; }
    public abstract string Name { get; }

    public virtual bool CanUseMagnetLinks => true;

    public string Url { get; set; }

    public TorrentScrapperServiceBase(
        string url)
    {
        Url = url;
    }

    public abstract string GetSearchUrl(
        string query);

    public abstract Task<string> GetStringAsync(
        string query);

    public abstract List<TorrentDto> GetTorrentsFromHtml(
        string content);

    public virtual List<string> GetLoginCommands(
        string username,
        string password)
    {
        return new List<string>();
    }

    public virtual string GetLoggedPattern()
    {
        return string.Empty;
    }

    public virtual Task<string> GetMagnetLinkFromHtmlAsync(
        string text,
        string cookies,
        string userAgent)
    {
        var regex = new Regex("href=['\"]magnet:(.*?)['\"]");
        var matches = regex.Matches(text);
        if (matches.Count <= 0)
        {
            return null;
        }

        return Task.FromResult($"magnet:{matches[0].Groups[1].Value}");
    }

    public virtual Task<string> GetTorrentLinkFromHtmlAsync(
        string text,
        string cookies,
        string userAgent)
    {
        var regex = new Regex("href=['\"](.*?)\\.torrent['\"]");
        var matches = regex.Matches(text);
        if (matches.Count <= 0)
        {
            return null;
        }

        return Task.FromResult($"{matches[0].Groups[1].Value}.torrent");
    }

    protected double ConvertSizeStringToNumber(
        string sizeString)
    {
        sizeString = sizeString.Replace("K", " K");
        sizeString = sizeString.Replace("k", " k");
        sizeString = sizeString.Replace("M", " M");
        sizeString = sizeString.Replace("m", " m");
        sizeString = sizeString.Replace("G", " G");
        sizeString = sizeString.Replace("g", " g");
        sizeString = sizeString.Replace(",", ".");
        sizeString = sizeString.RemoveDoubleSpacesAndTrim();

        var parts = sizeString.Split(' ');
        var number = Convert.ToDouble(parts[0], CultureInfo.InvariantCulture);
        var unit = parts[1];
        double torrentSize;

        switch (unit)
        {
            case "Kb":
            case "KB":
            case "Ko":
            case "KO":
                torrentSize = (double)(number * 1024);
                break;
            case "Mb":
            case "MB":
            case "Mo":
            case "MO":
                torrentSize = (double)(number * 1024 * 1024);
                break;
            case "Gb":
            case "GB":
            case "Go":
            case "GO":
                torrentSize = (double)(number * 1024 * 1024 * 1024);
                break;
            default:
                torrentSize = 0;
                break;
        }

        return torrentSize;
    }

    protected Tuple<string, string> GetParamsFromLink(
        string text)
    {
        var regex = new Regex("<a.*? href=['\"](.*?)['\"].*?>(.*?)</a>", RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
        var matches = regex.Matches(text);
        if (matches.Count < 1 || matches[0].Groups.Count < 3)
        {
            return null;
        }

        var link = matches[0].Groups[1].Value;
        var name = matches[0].Groups[2].Value.RemoveHtml();

        return Tuple.Create(name, link);
    }
}
