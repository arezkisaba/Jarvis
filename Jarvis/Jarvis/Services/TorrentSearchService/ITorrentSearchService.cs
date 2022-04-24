namespace Jarvis;

public interface ITorrentSearchService
{
    Task SearchTorrentsAsync(
        IEnumerable<Movie> movies,
        IEnumerable<TvShow> tvShows);
}
