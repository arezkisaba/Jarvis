namespace Jarvis.Features.Services.MediaRenamerService.Contracts;

public interface IMediaRenamerService
{
    Task RenameMovieAsync(
        string inputFolder,
        string titleForStorage,
        int? year);

    Task RenameEpisodeAsync(
        string inputFolder,
        string titleForStorage,
        int seasonNumber,
        int episodeNumber,
        string language);
}
