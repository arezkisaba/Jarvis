using Lib.ApiServices.Plex;
using Lib.ApiServices.Tmdb;
using Lib.Core;
using System.Reflection;

namespace Jarvis;

public class MediaCenterService : IMediaCenterService
{
    private readonly AppSettings _appSettings;
    private readonly Tokens _tokens;
    private readonly ILogger<MediaCenterService> _logger;
    private readonly IProcessManager _processManager;
    private readonly IPlexApiService _mediaCenterService;
    private readonly ITmdbApiService _mediaDatabaseService;
    private readonly IMediaService _mediaService;
    private bool _initializationDone = false;
    private IList<Movie> _movies;
    private IList<TvShow> _tvShows;

    public MediaCenterService(
        AppSettings appSettings,
        Tokens tokens,
        ILogger<MediaCenterService> logger,
        IProcessManager processManager,
        IPlexApiService mediaCenterService,
        ITmdbApiService mediaDatabaseService,
        IMediaService mediaService)
    {
        _appSettings = appSettings;
        _tokens = tokens;
        _logger = logger;
        _processManager = processManager;
        _mediaCenterService = mediaCenterService;
        _mediaDatabaseService = mediaDatabaseService;
        _mediaService = mediaService;
    }

    public Task<Tuple<IList<Movie>, IList<TvShow>>> FillMediasFromMediaCenterAndMediaDatabaseAsync()
    {
        return Task.Run(async () =>
        {
            try
            {
                if (!_initializationDone)
                {
                    await InitAsync();
                }

                _movies = new List<Movie>();
                _tvShows = new List<TvShow>();

                _logger.LogInformation("Providers data grabbing...");

                var sections = await _mediaCenterService.GetSectionsAsync();

                _logger.LogInformation("Movies grabbing...");

                var movieSection = sections.First(obj => obj.type == "movie");
                var moviesInMediaDatabase = await _mediaDatabaseService.GetMoviesInLibraryAsync();
                var moviesInMediaCenter = await _mediaCenterService.GetMoviesInLibraryAsync(movieSection.key.ToString());

                foreach (var movieInMediaDatabase in moviesInMediaDatabase)
                {
                    var movieInMediaCenter = moviesInMediaCenter.FirstOrDefault(obj => obj.Title == movieInMediaDatabase.Title);
                    var language = !string.IsNullOrWhiteSpace(movieInMediaDatabase.Language) ? movieInMediaDatabase.Language : "en";
                    var searchTitles = new List<string> { movieInMediaDatabase.Title.TransformForStorage() };
                    var secondLanguage = language == "en" ? "fr" : "en";
                    var movieTranslation = await _mediaDatabaseService.GetMovieTranslationsAsync(movieInMediaDatabase.Id, secondLanguage);

                    if (!string.IsNullOrWhiteSpace(movieTranslation?.FirstOrDefault()?.Title))
                    {
                        var secondTitle = movieTranslation.FirstOrDefault().Title?.TransformForStorage();
                        if (searchTitles.FirstOrDefault() != secondTitle)
                        {
                            searchTitles.Add(secondTitle);
                        }
                    }

                    var movie = new Movie(
                         movieInMediaDatabase.Id,
                         movieInMediaCenter?.Id,
                         movieInMediaDatabase.Title,
                         movieInMediaDatabase.Year,
                         language,
                         movieInMediaDatabase.IsCompleted,
                         movieInMediaCenter?.IsCompleted == true,
                         searchTitles);

                    _logger.LogInformation($"[{movie.Title}] added to list");
                    _movies.Add(movie);
                }

                _logger.LogInformation("Movies grabbed.");

                _logger.LogInformation("TvShows grabbing...");

                var showSection = sections.First(obj => obj.type == "show");
                var tvShowsInMediaDatabase = await _mediaDatabaseService.GetTvShowsInLibraryAsync();
                var tvShowsInMediaCenter = await _mediaCenterService.GetTvShowsInLibraryAsync(showSection.key.ToString());

                foreach (var tvShowInMediaDatabase in tvShowsInMediaDatabase)
                {
                    var tvShowInMediaCenter = tvShowsInMediaCenter.FirstOrDefault(obj => obj.Title == tvShowInMediaDatabase.Title);
                    var language = !string.IsNullOrWhiteSpace(tvShowInMediaDatabase.Language) ? tvShowInMediaDatabase.Language : "en";
                    var searchTitles = new List<string> { tvShowInMediaDatabase.Title.TransformForStorage() };

                    var secondLanguage = language == "en" ? "fr" : "en";
                    var tvShowTranslation = await _mediaDatabaseService.GetTvShowTranslationsAsync(tvShowInMediaDatabase.Id, secondLanguage);
                    if (!string.IsNullOrWhiteSpace(tvShowTranslation?.FirstOrDefault()?.Title))
                    {
                        var secondTitle = tvShowTranslation.FirstOrDefault().Title?.TransformForStorage();
                        if (searchTitles.FirstOrDefault() != secondTitle)
                        {
                            searchTitles.Add(secondTitle);
                        }
                    }

                    var tvShow = new TvShow(
                        tvShowInMediaDatabase.Id,
                        tvShowInMediaCenter?.Id,
                        tvShowInMediaDatabase.Title,
                        tvShowInMediaDatabase.Year,
                        language,
                        searchTitles);

                    foreach (var seasonCollectedInMediaDatabase in tvShowInMediaDatabase.Seasons)
                    {
                        var seasonInMediaCenter = tvShowInMediaCenter?.Seasons?.FirstOrDefault(obj => obj.Number == seasonCollectedInMediaDatabase.Number);

                        var season = new Season(
                            seasonInMediaCenter?.Id,
                            seasonCollectedInMediaDatabase.Number,
                            _appSettings.torrentConfig.language,
                            tvShow);

                        foreach (var episodeCollectedInMediaDatabase in seasonCollectedInMediaDatabase.Episodes)
                        {
                            var episodeInMediaCenter = seasonInMediaCenter?.Episodes?.FirstOrDefault(obj => obj.Number == episodeCollectedInMediaDatabase.Number);
                            var episode = new Episode(
                                episodeInMediaCenter?.Id,
                                episodeCollectedInMediaDatabase.Number,
                                episodeCollectedInMediaDatabase.IsCompleted,
                                episodeInMediaCenter?.IsCompleted == true,
                                season);
                            season.Episodes.Add(episode);
                        }

                        tvShow.Seasons.Add(season);
                    }

                    _logger.LogInformation($"[{tvShow.Title}] added to list");
                    _tvShows.Add(tvShow);

                    await _mediaService.CreateTvShowFoldersAsync(tvShow);
                }

                _logger.LogInformation("TvShows grabbed.");
                _logger.LogInformation("Providers data grabbed.");

                return Tuple.Create(_movies, _tvShows);
            }
            catch (Exception ex)
            {
                throw new JarvisException("Failed to grab providers data.", ex);
            }
        });
    }

    public Task SynchronizeWatchStatusBetweenMediaCenterAndMediaDatabaseAsync()
    {
        return Task.Run(async () =>
        {
            try
            {
                if (!_initializationDone)
                {
                    await InitAsync();
                }

                _logger.LogInformation("Watch status synchronization...");

                foreach (var movie in _movies)
                {
                    if (movie.IdMediaDatabase != null && movie.IsCompletedOnMediaCenter && !movie.IsCompletedOnMediaDatabase)
                    {
                        await _mediaDatabaseService.SetMovieWatchedAsync(movie.IdMediaDatabase, true);
                        _logger.LogInformation($"[{movie.Title}] set as watched in MediaDatabase");
                    }

                    if (movie.IdMediaCenter != null && movie.IsCompletedOnMediaDatabase && !movie.IsCompletedOnMediaCenter)
                    {
                        await _mediaCenterService.SetMovieWatchedAsync(movie.IdMediaCenter);
                        _logger.LogInformation($"[{movie.Title}] set as watched in MediaCenter");
                    }
                }

                foreach (var tvShow in _tvShows)
                {
                    foreach (var season in tvShow.Seasons)
                    {
                        foreach (var episode in season.Episodes)
                        {
                            var seasonFormatted = FormatHelper.GetSeasonShortName(season.Number);
                            var episodeFormatted = FormatHelper.GetEpisodeShortName(episode.Number);

                            if (tvShow.IdMediaDatabase != null && episode.IsCompletedOnMediaCenter && !episode.IsCompletedOnMediaDatabase)
                            {
                                await _mediaDatabaseService.SetEpisodeWatchedAsync(tvShow.IdMediaDatabase, season.Number, episode.Number, true);
                                _logger.LogInformation($"[{tvShow.Title} {seasonFormatted}{episodeFormatted}] set as watched in MediaDatabase");
                            }

                            if (episode.IdMediaCenter != null && episode.IsCompletedOnMediaDatabase && !episode.IsCompletedOnMediaCenter)
                            {
                                await _mediaCenterService.SetEpisodeWatchedAsync(episode.IdMediaCenter);
                                _logger.LogInformation($"[{tvShow.Title} {seasonFormatted}{episodeFormatted}] set as watched in MediaCenter");
                            }
                        }
                    }
                }

                _logger.LogInformation("Watch status synchronized.");
            }
            catch (Exception ex)
            {
                throw new JarvisException("Failed to synchronize watch status.", ex);
            }
        });
    }

    private Task InitAsync()
    {
        return Task.Run(() =>
        {
            var identity = Assembly.GetExecutingAssembly().GetName().Name;
            _mediaCenterService.SetPlexProperties(identity, identity, identity, "V1");

            _mediaDatabaseService.AuthenticationInformationsAvailable += (sender, e) =>
            {
                _logger.Log(LogLevel.Warning, $"Authentication URL : {e.AuthenticationUrl}");
                _processManager.StartWebBrowser(e.AuthenticationUrl);
            };

            _mediaDatabaseService.AuthenticationSuccessfull += async (sender, e) =>
            {
                _tokens.TmdbSessionId = e.SessionId;
                await FileWrapper.WriteObjectAsync(StorageHelper.GetTokensFile(_appSettings.appdataDirectory), _tokens, ExchangeFormat.Json);
            };

            _initializationDone = true;
        });
    }
}
