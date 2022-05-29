namespace Jarvis.Features.Services.MediaNamingService.Contracts;

public interface IMediaNamingService
{
    string[] GetPossibleMediaTitles(
        string torrentTitle);

    string GetDisplayNameForMovie(
        string movieTitle);

    string GetDisplayNameForEpisode(
        string tvShowTitle,
        int seasonNumber,
        int episodeNumber);

    string GetShortNameForSeason(
        int seasonNumber);

    string GetShortNameForEpisode(
        int episodeNumber);
}
