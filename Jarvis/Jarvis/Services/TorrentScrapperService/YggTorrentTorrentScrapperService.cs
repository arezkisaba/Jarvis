using Lib.Core;
using System.IO;
using System.Text.RegularExpressions;

namespace Jarvis;

public class YggTorrentTorrentScrapperService : TorrentScrapperServiceBase
{
    public override bool IsActive => true;
    public override string Name => "YggTorrent";

    public override bool CanUseMagnetLinks => false;

    public YggTorrentTorrentScrapperService(
        string url)
        : base(url)
    {
    }

    public override string GetSearchUrl(
        string query)
    {
        return $"{Url}/engine/search?name={query}&do=search";
    }

    public override Task<string> GetStringAsync(
        string query)
    {
        throw new NotImplementedException();
    }

    public override List<TorrentDto> GetTorrentsFromHtml(
        string content)
    {
        content = content.RemoveCarriageReturnAndOtherFuckingCharacters();

        var torrents = new List<TorrentDto>();
        var rows = new Regex("<tr.*?>(.*?)</tr>").Matches(content)
            .Select(obj => obj.Groups[0].Value).Where(obj => obj.StartsWith("<tr><td><div class=\"hidden\">"))
            .ToList();

        foreach (var row in rows)
        {
            var columns = new Regex("<td.*?>(.*?)</td>").Matches(row)
                .Select(obj => obj.Groups[1].Value)
                .ToList();

            var nameAndLink = GetParamsFromLink(columns.ElementAt(1));
            var valueSize = columns.ElementAt(5).RemoveHtml();
            var valueSeeds = columns.ElementAt(7).RemoveHtml();

            if (!string.IsNullOrWhiteSpace(nameAndLink.Item2))
            {
                torrents.Add(new TorrentDto
                {
                    DescriptionUrl = $"{nameAndLink.Item2}",
                    Name = nameAndLink.Item1,
                    Provider = Name,
                    Seeds = Convert.ToInt32(valueSeeds),
                    Size = ConvertSizeStringToNumber(valueSize)
                });
            }
        }

        return torrents;
    }

    public override List<string> GetLoginCommands(
        string username,
        string password)
    {
        return new List<string>
            {
                $"$('input[name=id]').val('{username}')",
                $"$('input[name=pass]').val('{password}')",
                $"$('#user-login').submit()"
            };
    }

    public override string GetLoggedPattern()
    {
        return "messages_count";
    }

    public override async Task<string> GetTorrentLinkFromHtmlAsync(
        string text,
        string cookies,
        string userAgent)
    {
        var regex = new Regex("href=\"/(engine/download_torrent\\?id=.*?)\" title");
        var matches = regex.Matches(text);
        if (matches.Count <= 0)
        {
            return null;
        }

        var headers = new Dictionary<string, string>
        {
            { "Cookie", cookies },
            { "User-Agent", userAgent }
        };

        var httpService = new HttpService(Url, headers: headers);
        var bytes = await httpService.GetByteArrayAsync(matches[0].Groups[1].Value);
        var filePath = $"{Environment.GetEnvironmentVariable("AppData")}\\{RandomHelper.GetRandomAlphaLowerCaseString(10)}.torrent";
        await File.WriteAllBytesAsync(filePath, bytes);
        return filePath;
    }
}
