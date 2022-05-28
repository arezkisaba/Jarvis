namespace Jarvis.Technical.Helpers;

public static class FormatHelper
{
    public static string GetSeasonShortName(
        int seasonNumber)
    {
        var suffix = seasonNumber < 10 ? "0" : string.Empty;
        return $"S{suffix}{seasonNumber}";
    }

    public static string GetSeasonLongName(
        int seasonNumber)
    {
        return $"Saison {seasonNumber}";
    }

    public static string GetEpisodeShortName(
        int episodeNumber)
    {
        var suffix = episodeNumber < 10 ? "0" : string.Empty;
        return $"E{suffix}{episodeNumber}";
    }

    public static string GetEpisodeLongName(
        int episodeNumber)
    {
        return $"Episode {episodeNumber}";
    }

    public static IEnumerable<string> GetEpisodePossibleShortNames(
        int seasonNumber,
        int episodeNumber)
    {
        var episodeShortNames = new List<string>
        {
            seasonNumber + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "X" + episodeNumber,
            "X" + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "X " + episodeNumber,
            "X " + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "E" + episodeNumber,
            "E" + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "E " + episodeNumber,
            "E " + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "EP" + episodeNumber,
            "EP" + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "EP " + episodeNumber,
            "EP " + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "EPISODE" + episodeNumber,
            "EPISODE" + (episodeNumber < 10 ? "0" : "") + episodeNumber,
            "EPISODE " + episodeNumber,
            "EPISODE " + (episodeNumber < 10 ? "0" : "") + episodeNumber
        };
        return episodeShortNames;
    }

    public static string GetExtensionFromNameOrPath(
        string nameOrPath)
    {
        if (!nameOrPath.Contains("."))
        {
            return string.Empty;
        }

        return nameOrPath[(nameOrPath.LastIndexOf(".") + 1)..];
    }

    public static string GetStringWithUnitFromSize(
        double size)
    {
        var unit = "B";
        var coef = 1024d;

        if (size / coef > 1)
        {
            size /= coef;
            unit = "KB";
        }

        if (size / coef > 1)
        {
            size /= coef;
            unit = "MB";
        }

        if (size / coef > 1)
        {
            size /= coef;
            unit = "GB";
        }

        if (size / coef > 1)
        {
            size /= coef;
            unit = "TB";
        }

        return $"{size.ToString("0.##")} {unit}";
    }
}
