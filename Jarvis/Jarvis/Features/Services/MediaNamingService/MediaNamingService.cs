using Jarvis.Features.Services.MediaNamingService.Contracts;

namespace Jarvis.Features.Services.MediaNamingService;

public class MediaNamingService : IMediaNamingService
{
    public string[] GetPossibleMediaTitles(
        string foundTitle)
    {
        var indexes = new List<int>();
        var titles = new List<string>()
        {
            foundTitle
        };
        
        for (var i = foundTitle.Length - 1; i >= 0; i--)
        {
            if (foundTitle[i] == ' ' || foundTitle[i] == '.')
            {
                indexes.Add(i);
            }
        }

        foreach (var index in indexes)
        {
            titles.Add(foundTitle[..index]);
        }

        return titles.ToArray();
    }

    public string GetDisplayNameForMovie(
        string movieTitle)
    {
        return $"{movieTitle}";
    }

    public string GetDisplayNameForSeason(
        string tvShowTitle,
        int seasonNumber)
    {
        var seasonNumberPrefix = seasonNumber < 10 ? "0" : string.Empty;
        return $"{tvShowTitle} S{seasonNumberPrefix}{seasonNumber}";
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

    public string GetShortNameForSeason(
        int seasonNumber)
    {
        return GetShortNameForLetter("S", seasonNumber);
    }

    public string GetShortNameForEpisode(
        int episodeNumber)
    {
        return GetShortNameForLetter("E", episodeNumber);
    }

    #region Private use

    private string GetShortNameForLetter(
        string letter,
        int number)
    {
        var prefix = number < 10 ? "0" : string.Empty;
        return $"{letter}{prefix}{number}";
    }

    #endregion
}
