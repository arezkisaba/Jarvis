using Jarvis.Features.Services.TorrentScrapperService.Models;
using Lib.Core;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Jarvis.Features.Services.TorrentScrapperService.Providers.Bases;

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

    public abstract Task<List<TorrentDto>> GetResultsAsync(
        string query);

    public abstract Task<string> GetDownloadLinkAsync(
        string descriptionUrl,
        string cookies,
        string userAgent);

    #region Private use

    protected virtual Task<string> GetMagnetLinkFromHtmlAsync(
        string text,
        string cookies,
        string userAgent)
    {
        var regex = new Regex("['\"](magnet:(.*?))['\"]");
        var matches = regex.Matches(text);
        if (matches.Count <= 0)
        {
            return null;
        }

        return Task.FromResult($"{matches[0].Groups[1].Value}");
    }

    protected virtual Task<string> GetTorrentLinkFromHtmlAsync(
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

    protected virtual List<string> GetLoginCommands(
        string username,
        string password)
    {
        return new List<string>();
    }

    protected virtual string GetLoggedPattern()
    {
        return string.Empty;
    }

    protected long ConvertSizeStringToNumber(
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
                torrentSize = number * 1024;
                break;
            case "Mb":
            case "MB":
            case "Mo":
            case "MO":
                torrentSize = number * 1024 * 1024;
                break;
            case "Gb":
            case "GB":
            case "Go":
            case "GO":
                torrentSize = number * 1024 * 1024 * 1024;
                break;
            default:
                torrentSize = 0;
                break;
        }

        return (long)Math.Round(torrentSize);
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

    #endregion
}
