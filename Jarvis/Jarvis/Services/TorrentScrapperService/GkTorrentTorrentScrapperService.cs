using Lib.Core;
using System.Text.RegularExpressions;

namespace Jarvis;

public class GkTorrentTorrentScrapperService : TorrentScrapperServiceBase
{
    public override bool IsActive => true;
    public override string Name => "GkTorrent";

    public GkTorrentTorrentScrapperService(
        string url)
        : base(url)
    {
    }

    public override string GetSearchUrl(
        string query)
    {
        return $"{Url}/recherche/{query}";
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
                torrents.Add(new TorrentDto
                {
                    DescriptionUrl = $"{Url}{nameAndLink.Item2}",
                    Name = nameAndLink.Item1,
                    Provider = Name,
                    Seeds = Convert.ToInt32(valueSeeds),
                    Size = ConvertSizeStringToNumber(valueSize)
                });
            }
        }

        return torrents;
    }
}
