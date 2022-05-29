using Jarvis.Features.Services.MediaNamingService.Contracts;

namespace Jarvis.Features.Services.MediaNamingService;

public class MediaNamingService : IMediaNamingService
{
    public string[] GetPossibleMovieTitles(
        string torrentTitle)
    {
        var indexes = new List<int>();
        var titles = new List<string>()
        {
            torrentTitle
        };
        
        for (var i = torrentTitle.Length - 1; i >= 0; i--)
        {
            if (torrentTitle[i] == ' ' || torrentTitle[i] == '.')
            {
                indexes.Add(i);
            }
        }

        foreach (var index in indexes)
        {
            titles.Add(torrentTitle[..index]);
        }

        return titles.ToArray();
    }

    public string GetDisplayNameForMovie(
        string movieTitle)
    {
        return $"{movieTitle}";
    }

    public string GetDisplayNameForEpisode(
        string tvShowTitle,
        int seasonNumber,
        int episodeNumber)
    {
        var seasonNumberPrefix = seasonNumber < 10 ? "0" : string.Empty;
        var episodeNumberPrefix = episodeNumber < 10 ? "0" : string.Empty;
        return $"{tvShowTitle} S{seasonNumberPrefix}{seasonNumber}E{episodeNumberPrefix}{episodeNumber}";
    }
}
