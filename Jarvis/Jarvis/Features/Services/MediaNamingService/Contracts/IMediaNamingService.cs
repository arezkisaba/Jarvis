namespace Jarvis.Features.Services.MediaNamingService.Contracts;

public interface IMediaNamingService
{
    string[] GetPossibleMovieTitles(
        string torrentTitle);

    string GetDisplayNameForMovie(
        string movieTitle);

    string GetDisplayNameForEpisode(
        string tvShowTitle,
        int seasonNumber,
        int episodeNumber);
}
