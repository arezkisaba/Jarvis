namespace Jarvis;

public interface IMediaService
{
    void CreateDirectoriesIfDoesntExists();

    Task CreateTvShowFoldersAsync(
        TvShow tvShow);

    Task<IEnumerable<Movie>> GetMissingMoviesOnDiskAsync(
        IEnumerable<Movie> movies);

    Task<IEnumerable<Episode>> GetMissingEpisodesForSeasonOnDiskAsync(
        Season season,
        string language);

    Task RenameMovieOnDiskAsync(
        string baseFolder,
        string movieStorageTitle,
        int? movieYear);

    Task RenameEpisodeOnDiskAsync(
        string baseFolder,
        string tvShowStorageTitle,
        int seasonNumber,
        int episodeNumber,
        string language);

    string GetDownloadFolder();
}
