using Jarvis.Features.Services.MediaDatabaseService.Contracts;
using Jarvis.Features.Services.MediaDatabaseService.Models;
using Lib.ApiServices.Tmdb;

namespace Jarvis.Features.Services.MediaDatabaseService;

public class MediaDatabaseService : IMediaDatabaseService
{
    private readonly ITmdbApiService _tmdbApiService;

    public event EventHandler<string> AuthenticationSuccessfull;

    public event EventHandler<string> AuthenticationInformationsAvailable;

    public MediaDatabaseService(
        ITmdbApiService tmdbApiService)
    {
        _tmdbApiService = tmdbApiService;
        _tmdbApiService.AuthenticationSuccessfull += (sender, e) =>
        {
            AuthenticationSuccessfull?.Invoke(sender, e.SessionId);
        };
        _tmdbApiService.AuthenticationInformationsAvailable += (sender, e) =>
        {
            AuthenticationInformationsAvailable?.Invoke(sender, e.AuthenticationUrl);
        };
    }

    public async Task<IEnumerable<Movie>> SearchMovieAsync(
        string query)
    {
        var movies = await _tmdbApiService.SearchMovieAsync(query);
        return movies.Select(obj => new Movie(
            Title: obj.Title,
            Year: obj.Year)
        );
    }

    public async Task<IEnumerable<TvShow>> SearchTvShowAsync(
        string query)
    {
        var tvShows = await _tmdbApiService.SearchTvShowAsync(query);
        return tvShows.Select(obj => new TvShow(
            Id: obj.Id,
            Title: obj.Title)
        );
    }

    public async Task<IEnumerable<Season>> GetSeasonsAsync(
        string tvShowId)
    {
        var seasons = await _tmdbApiService.GetSeasonsAsync(tvShowId);
        return seasons.Select(obj => new Season(
            Number: obj.Number)
        );
    }

    public async Task<IEnumerable<Episode>> GetEpisodesAsync(
        string tvShowId,
        int seasonNumber)
    {
        var episodes = await _tmdbApiService.GetEpisodesAsync(tvShowId, seasonNumber);
        return episodes.Select(obj => new Episode(
            Number: obj.Number)
        );
    }
}
