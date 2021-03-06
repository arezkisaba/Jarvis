using Jarvis.Features.Services.TorrentScrapperService.Models;
using Jarvis.Features.Services.TorrentScrapperService.Providers.Bases;
using Lib.Core;
using System.Text.RegularExpressions;

namespace Jarvis.Features.Services.TorrentScrapperService.Providers;

public class ZeTorrentsTorrentScrapperService : TorrentScrapperServiceBase
{
    public override bool IsActive => true;
    public override string Name => "ZeTorrents";

    public ZeTorrentsTorrentScrapperService(
        string url)
        : base(url)
    {
    }

    public override async Task<List<TorrentDto>> GetResultsAsync(
        string query)
    {
        var httpService = new HttpService(Url);
        var content = await httpService.GetStringAsync($"recherche/{query}");

        content = content.RemoveCarriageReturnAndOtherFuckingCharacters();

        var torrents = new List<TorrentDto>();
        var rows = new Regex("<tr.*?>(.*?)</tr>").Matches(content)
            .Select(obj => obj.Groups[0].Value).Where(obj => !obj.Contains("<th") && !obj.Contains("Pas de torrents"))
            .ToList();

        foreach (var row in rows)
        {
            var columns = new Regex("<td.*?>(.*?)</td>").Matches(row)
                .Select(obj => obj.Groups[1].Value)
                .ToList();

            var nameAndLink = GetParamsFromLink(columns.ElementAt(0));
            var valueSize = columns.ElementAt(1).RemoveHtml();
            var valueSeeds = columns.ElementAt(2).RemoveHtml();

            if (!string.IsNullOrWhiteSpace(nameAndLink.Item2))
            {
                torrents.Add(new TorrentDto(
                    DescriptionUrl: $"{Url}{nameAndLink.Item2}",
                    Name: nameAndLink.Item1,
                    Provider: Name,
                    Seeds: Convert.ToInt32(valueSeeds),
                    Size: ConvertSizeStringToNumber(valueSize))
                );
            }
        }

        return torrents;
    }

    public override async Task<string> GetDownloadLinkAsync(
        string descriptionUrl,
        string cookies,
        string userAgent)
    {
        var httpService = new HttpService(descriptionUrl);
        var content = await httpService.GetStringAsync(string.Empty);
        return await GetMagnetLinkFromHtmlAsync(content, cookies, userAgent);
    }
}
