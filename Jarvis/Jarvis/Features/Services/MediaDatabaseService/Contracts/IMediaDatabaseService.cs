using Jarvis.Features.Services.MediaDatabaseService.Models;

namespace Jarvis.Features.Services.MediaDatabaseService.Contracts;

public interface IMediaDatabaseService
{
    event EventHandler<string> AuthenticationSuccessfull;

    event EventHandler<string> AuthenticationInformationsAvailable;

    Task<IEnumerable<Movie>> SearchMovieAsync(
        string query);

    Task<IEnumerable<TvShow>> SearchTvShowAsync(
        string query);

    Task<IEnumerable<Episode>> GetEpisodesAsync(
        string tvShowId,
        int seasonNumber);

    Task<IEnumerable<Season>> GetSeasonsAsync(
        string tvShowId);
}