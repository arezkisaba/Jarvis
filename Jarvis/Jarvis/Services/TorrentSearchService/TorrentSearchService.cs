using Lib.Core;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Jarvis;

public class TorrentSearchService : ITorrentSearchService
{
    private readonly AppSettings _appSettings;
    private readonly ILogger<TorrentSearchService> _logger;
    private readonly ITorrentDownloaderService _downloaderService;
    private readonly IMediaService _mediaService;

    public TorrentSearchService(
        AppSettings appSettings,
        ILogger<TorrentSearchService> logger,
        ITorrentDownloaderService downloaderService,
        IMediaService mediaService)
    {
        _appSettings = appSettings;
        _logger = logger;
        _downloaderService = downloaderService;
        _mediaService = mediaService;
    }

    public async Task SearchTorrentsAsync(
        IEnumerable<Movie> movies,
        IEnumerable<TvShow> tvShows)
    {
        try
        {
            _logger.LogInformation("Torrent search...");

            var missingMoviesOnDisk = await _mediaService.GetMissingMoviesOnDiskAsync(movies);
            foreach (var movie in missingMoviesOnDisk)
            {
                var movieTorrent = await TryGetBestTorrentForMovieAsync(movie.SearchTitles, movie.Year, _appSettings.torrentConfig.language);
                if (movieTorrent != null)
                {
                    var download = new GetModelResponse.DownloadsSectionItem(DownloadMode.Movie, movieTorrent.Name.TransformForStorage(), movieTorrent.Provider, movieTorrent.Url, _mediaService.GetDownloadFolder(), _appSettings.torrentConfig.language, movieTorrent.Seeds, movieTorrent.Size, movie.StorageTitle, movie.Year.Value);
                    _downloaderService.Downloads.Add(download);
                    _logger.LogInformation($"[{download.GetDisplayTitle()}] ({movieTorrent.Name}) found.");
                }
                else
                {
                    _logger.LogInformation($"[{movie.Title}] not found");
                }

                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }

            foreach (var tvShow in tvShows)
            {
                foreach (var season in tvShow.Seasons)
                {
                    var missingEpisodesForSeasonOnDisk = await _mediaService.GetMissingEpisodesForSeasonOnDiskAsync(season, season.Language);
                    if (!missingEpisodesForSeasonOnDisk.Any())
                    {
                        continue;
                    }

                    var isSeasonTorrentFoundAndAllEpisodeMissing = false;
                    var areAllEpisodeMissing = season.EpisodesCollectedCount == missingEpisodesForSeasonOnDisk.Count();
                    if (areAllEpisodeMissing)
                    {
                        var seasonTorrent = await TryGetBestTorrentForSeasonAsync(season.TvShow.SearchTitles, season.Number, season.Language);
                        if (seasonTorrent != null)
                        {
                            isSeasonTorrentFoundAndAllEpisodeMissing = true;
                            var download = new GetModelResponse.DownloadsSectionItem(DownloadMode.Season, seasonTorrent.Name.TransformForStorage(), seasonTorrent.Provider, seasonTorrent.Url, _mediaService.GetDownloadFolder(), season.Language, seasonTorrent.Seeds, seasonTorrent.Size, tvShow.StorageTitle, season.Number, season.Episodes.Count);
                            _downloaderService.Downloads.Add(download);
                            _logger.LogInformation($"[{download.GetDisplayTitle()}] ({seasonTorrent.Name}) found.");
                        }
                        else
                        {
                            isSeasonTorrentFoundAndAllEpisodeMissing = false;
                            _logger.LogInformation($"[{tvShow.Title} {FormatHelper.GetSeasonShortName(season.Number)} {season.Language}] not found");
                        }

                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                    }

                    if (!isSeasonTorrentFoundAndAllEpisodeMissing)
                    {
                        Episode lastEpisode = null;
                        TorrentDto lastEpisodeTorrent = null;

                        foreach (var missingEpisodeForSeasonOnDisk in missingEpisodesForSeasonOnDisk)
                        {
                            if (lastEpisodeTorrent == null && lastEpisode != null && lastEpisode.Season.Number == missingEpisodeForSeasonOnDisk.Season.Number)
                            {
                                _logger.LogInformation($"[{tvShow.Title} {FormatHelper.GetSeasonShortName(season.Number)}{FormatHelper.GetEpisodeShortName(missingEpisodeForSeasonOnDisk.Number)} {season.Language}] skipped");
                                continue;
                            }

                            var episodeTorrent = await TryGetBestTorrentForEpisodeAsync(missingEpisodeForSeasonOnDisk.Season.TvShow.SearchTitles, missingEpisodeForSeasonOnDisk.Season.Number, missingEpisodeForSeasonOnDisk.Number, season.Language);
                            if (episodeTorrent != null)
                            {
                                var download = new GetModelResponse.DownloadsSectionItem(DownloadMode.Episode, episodeTorrent.Name.TransformForStorage(), episodeTorrent.Provider, episodeTorrent.Url, _mediaService.GetDownloadFolder(), season.Language, episodeTorrent.Seeds, episodeTorrent.Size, tvShow.StorageTitle, season.Number, season.Episodes.Count, missingEpisodeForSeasonOnDisk.Number);
                                _downloaderService.Downloads.Add(download);
                                _logger.LogInformation($"[{download.GetDisplayTitle()}] ({episodeTorrent.Name}) found.");
                            }
                            else
                            {
                                _logger.LogInformation($"[{tvShow.Title} {FormatHelper.GetSeasonShortName(season.Number)}{FormatHelper.GetEpisodeShortName(missingEpisodeForSeasonOnDisk.Number)} {season.Language}] not found");
                            }

                            lastEpisode = missingEpisodeForSeasonOnDisk;
                            lastEpisodeTorrent = episodeTorrent;

                            await Task.Delay(TimeSpan.FromMilliseconds(500));
                        }
                    }
                }
            }

            _logger.LogInformation("Torrent search finished.");
        }
        catch (Exception ex)
        {
            throw new JarvisException("Failed to search torrents.", ex);
        }
    }

    private async Task<TorrentDto> TryGetBestTorrentForMovieAsync(
        IEnumerable<string> searchTitles,
        int? movieYear,
        string language)
    {
        try
        {
            var queries = new List<TorrentSearchQueryModel>();

            foreach (var searchTitle in searchTitles)
            {
                var query1 = $"{GetSearchTitleForTorrentProviders(searchTitle)}";
                var result1 = await StartWebProxyAsync($"\"torrents?searchTitle={searchTitle}&query={query1}&providers={_appSettings.torrentConfig.providers}\"");
                if (!string.IsNullOrWhiteSpace(result1))
                {
                    queries.AddRange(Serializer.JsonDeserialize<IEnumerable<TorrentSearchQueryModel>>(result1));
                }
            }

            foreach (var query in queries)
            {
                if (query.Results != null && query.Results.Any())
                {
                    RemoveBadTorrents(query, null, null, null, null, DownloadMode.Movie, language);
                }
            }

            var bestTorrent = GetBestTorrent(queries.Where(obj => obj.Results.Any()).ToList(), movieYear);
            if (bestTorrent != null)
            {
                bestTorrent.Url = await StartWebProxyAsync($"\"torrent?provider={bestTorrent.Provider}&descriptionUrl={bestTorrent.DescriptionUrl}\"");
            }

            return bestTorrent;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<TorrentDto> TryGetBestTorrentForSeasonAsync(
        IEnumerable<string> searchTitles,
        int seasonNumber,
        string language)
    {
        try
        {
            var seasonShortName = FormatHelper.GetSeasonShortName(seasonNumber);
            var seasonLongName = FormatHelper.GetSeasonLongName(seasonNumber);
            var queries = new List<TorrentSearchQueryModel>();

            foreach (var searchTitle in searchTitles)
            {
                var query1 = $"{GetSearchTitleForTorrentProviders(searchTitle)} {seasonShortName}";
                var result1 = await StartWebProxyAsync($"\"torrents?searchTitle={searchTitle}&query={query1}&providers={_appSettings.torrentConfig.providers}\"");

                if (!string.IsNullOrWhiteSpace(result1))
                {
                    queries.AddRange(Serializer.JsonDeserialize<IEnumerable<TorrentSearchQueryModel>>(result1));
                }

                var query2 = $"{GetSearchTitleForTorrentProviders(searchTitle)} {seasonLongName}";
                var result2 = await StartWebProxyAsync($"\"torrents?searchTitle={searchTitle}&query={query2}&providers={_appSettings.torrentConfig.providers}\"");

                if (!string.IsNullOrWhiteSpace(result2))
                {
                    queries.AddRange(Serializer.JsonDeserialize<IEnumerable<TorrentSearchQueryModel>>(result2));
                }
            }

            foreach (var query in queries)
            {
                if (query.Results != null && query.Results.Any())
                {
                    RemoveBadTorrents(query, seasonShortName, seasonLongName, null, null, DownloadMode.Season, language);
                }
            }

            var bestTorrent = GetBestTorrent(queries.Where(obj => obj.Results.Any()).ToList());
            if (bestTorrent != null)
            {
                bestTorrent.Url = await StartWebProxyAsync($"\"torrent?provider={bestTorrent.Provider}&descriptionUrl={bestTorrent.DescriptionUrl}\"");
            }

            return bestTorrent;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<TorrentDto> TryGetBestTorrentForEpisodeAsync(
        IEnumerable<string> searchTitles,
        int seasonNumber,
        int episodeNumber,
        string language)
    {
        try
        {
            var seasonShortName = FormatHelper.GetSeasonShortName(seasonNumber);
            var seasonLongName = FormatHelper.GetSeasonLongName(seasonNumber);
            var episodeShortName = FormatHelper.GetEpisodeShortName(episodeNumber);
            var episodeLongName = FormatHelper.GetEpisodeLongName(episodeNumber);
            var queries = new List<TorrentSearchQueryModel>();

            foreach (var searchTitle in searchTitles)
            {
                var query1 = $"{GetSearchTitleForTorrentProviders(searchTitle)} {seasonShortName}{episodeShortName}";
                var parameters = $"\"torrents?searchTitle={searchTitle}&query={query1}&providers={_appSettings.torrentConfig.providers}\"";
                var result1 = await StartWebProxyAsync(parameters);

                if (!string.IsNullOrWhiteSpace(result1))
                {
                    queries.AddRange(Serializer.JsonDeserialize<IEnumerable<TorrentSearchQueryModel>>(result1));
                }

                var query2 = $"{GetSearchTitleForTorrentProviders(searchTitle)} {seasonLongName} {episodeLongName}";
                var result2 = await StartWebProxyAsync($"\"torrents?searchTitle={searchTitle}&query={query2}&providers={_appSettings.torrentConfig.providers}\"");

                if (!string.IsNullOrWhiteSpace(result2))
                {
                    queries.AddRange(Serializer.JsonDeserialize<IEnumerable<TorrentSearchQueryModel>>(result2));
                }
            }

            foreach (var query in queries)
            {
                if (query.Results != null && query.Results.Any())
                {
                    RemoveBadTorrents(query, seasonShortName, seasonLongName, episodeShortName, episodeLongName, DownloadMode.Episode, language);
                }
            }

            var bestTorrent = GetBestTorrent(queries.Where(obj => obj.Results.Any()).ToList());
            if (bestTorrent != null)
            {
                bestTorrent.Url = await StartWebProxyAsync($"\"torrent?provider={bestTorrent.Provider}&descriptionUrl={bestTorrent.DescriptionUrl}\"");
            }

            return bestTorrent;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<string> StartWebProxyAsync(
        string parameters)
    {
        string json = null;
        var tcs = new TaskCompletionSource<string>();
        var webProxyProcess = new Process();

        void OnWebProxyProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data == null)
            {
                return;
            }

            webProxyProcess.OutputDataReceived -= OnWebProxyProcessOutputDataReceived;
            json = ConversionHelper.Base64ToString(e.Data);
            tcs.SetResult(json);
        }

        webProxyProcess.StartInfo.WorkingDirectory = new FileInfo(_appSettings.torrentConfig.webProxyPath).Directory.FullName;
        webProxyProcess.StartInfo.FileName = $"{_appSettings.torrentConfig.webProxyPath}";
        webProxyProcess.StartInfo.Arguments = $"{parameters}";
        webProxyProcess.StartInfo.UseShellExecute = false;
        webProxyProcess.StartInfo.RedirectStandardOutput = true;
        webProxyProcess.StartInfo.RedirectStandardError = true;
        webProxyProcess.EnableRaisingEvents = true;
        webProxyProcess.OutputDataReceived += OnWebProxyProcessOutputDataReceived;
        webProxyProcess.Start();
        webProxyProcess.BeginOutputReadLine();
        webProxyProcess.BeginErrorReadLine();
        return await tcs.Task;
    }

    private void RemoveBadTorrents(
        TorrentSearchQueryModel query,
        string seasonShortName,
        string seasonLongName,
        string episodeShortName,
        string episodeLongName,
        DownloadMode downloadMode,
        string language)
    {
        query.Results.RemoveAll(torrent => _appSettings.torrentConfig.excludedPatterns.Any(excludedPattern => torrent.Name.RemoveSpecialCharacters(" ").ContainsIgnoreCase($" {excludedPattern}")));
        query.Results.RemoveAll(torrent => !torrent.Name.RemoveSpecialCharacters(" ").ContainsIgnoreCase($"{language}"));
        query.Results.RemoveAll(torrent => torrent.Seeds < _appSettings.torrentConfig.minSeeders);
        query.Results.RemoveAll(torrent => !MatchMediaTitleAndTorrentName(query.SearchTitle.RemoveSpecialCharacters(" "), torrent.Name.RemoveSpecialCharacters(" ")));

        if (downloadMode == DownloadMode.Movie)
        {
            query.Results.RemoveAll(torrent => torrent.Size < _appSettings.torrentConfig.mediaConfig.movieMinSize || torrent.Size > _appSettings.torrentConfig.mediaConfig.movieMaxSize);

            query.Results.RemoveAll(torrent => HasSeasonInFilename($"{torrent.Name.RemoveSpecialCharacters(" ")}"));

            query.Results.RemoveAll(torrent => HasEpisodeInFilename($"{torrent.Name.RemoveSpecialCharacters(" ")}"));
        }
        else if (downloadMode == DownloadMode.Season)
        {
            query.Results.RemoveAll(torrent => torrent.Size < _appSettings.torrentConfig.mediaConfig.seasonMinSize && torrent.Size > _appSettings.torrentConfig.mediaConfig.seasonMaxSize);

            query.Results.RemoveAll(torrent =>
            {
                var hasShortCombination = torrent.Name.RemoveSpecialCharacters(" ").ContainsIgnoreCase($"{seasonShortName}");
                var hasLongCombination = torrent.Name.RemoveSpecialCharacters(" ").ContainsIgnoreCase($"{seasonLongName}");
                return !hasShortCombination && !hasLongCombination;
            });

            query.Results.RemoveAll(torrent => HasEpisodeInFilename($"{torrent.Name.RemoveSpecialCharacters(" ")}"));
        }
        else if (downloadMode == DownloadMode.Episode)
        {
            query.Results.RemoveAll(torrent => torrent.Size < _appSettings.torrentConfig.mediaConfig.episodeMinSize && torrent.Size > _appSettings.torrentConfig.mediaConfig.episodeMaxSize);

            query.Results.RemoveAll(torrent =>
            {
                var hasShortCombination = torrent.Name.RemoveSpecialCharacters(" ").ContainsIgnoreCase($"{seasonShortName}") && torrent.Name.RemoveSpecialCharacters(" ").ContainsIgnoreCase($"{episodeShortName}");
                var hasLongCombination = torrent.Name.RemoveSpecialCharacters(" ").ContainsIgnoreCase($"{seasonLongName}") && torrent.Name.RemoveSpecialCharacters(" ").ContainsIgnoreCase($"{episodeLongName}");
                return !hasShortCombination && !hasLongCombination;
            });
        }
    }

    private TorrentDto GetBestTorrent(
        IEnumerable<TorrentSearchQueryModel> queries,
        int? year = null)
    {
        TorrentDto bestTorrent = null;
        var allResults = new List<TorrentDto>();
        foreach (var query in queries)
        {
            allResults.AddRange(query.Results);
        }

        foreach (var query in queries)
        {
            var canUseYearAsFilter = false;
            var canUseStartsWithAsFilter = false;

            if (year.HasValue)
            {
                var resultsMatchingWithYear = allResults.Where(obj => obj.Name.Contains(year.Value.ToString())).ToList();
                canUseYearAsFilter = resultsMatchingWithYear.Any();
                if (canUseYearAsFilter)
                {
                    query.Results.RemoveAll(obj => resultsMatchingWithYear.All(obj2 => obj2.DescriptionUrl != obj.DescriptionUrl));
                }
            }

            if (!canUseYearAsFilter)
            {
                var resultsMatchingWithStartsWith = allResults.Where(obj =>
                {
                    var torrentNameFormatted = obj.Name.RemoveSpecialCharacters(" ").RemoveDoubleSpacesAndTrim();
                    var queryStringFormatted = query.QueryString.RemoveSpecialCharacters(" ").RemoveDoubleSpacesAndTrim();
                    if (torrentNameFormatted.Replace(" FINAL", string.Empty).StartsWithIgnoreCase($"{queryStringFormatted} "))
                    {
                        if (year.HasValue)
                        {
                            var yearsFoundInTorrentName = GetYearsInText(torrentNameFormatted);
                            if (yearsFoundInTorrentName.Any() && yearsFoundInTorrentName.Any(obj => obj == year.Value))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }

                    return false;
                }).ToList();
                canUseStartsWithAsFilter = resultsMatchingWithStartsWith.Any();
                if (canUseStartsWithAsFilter)
                {
                    query.Results.RemoveAll(obj => resultsMatchingWithStartsWith.All(obj2 => obj2.DescriptionUrl != obj.DescriptionUrl));
                }
            }
        }

        foreach (var query in queries)
        {
            foreach (var torrent in query.Results)
            {
                if (bestTorrent == null || torrent.Size > bestTorrent.Size)
                {
                    bestTorrent = torrent;
                }
            }
        }

        return bestTorrent;
    }

    private IEnumerable<int> GetYearsInText(
        string text)
    {
        var matches = new Regex("[0-9]{4}").Matches(text);
        if (matches != null)
        {
            return matches.Select(obj => int.Parse(obj.Value)).ToList();
        }

        return Array.Empty<int>();
    }

    private bool HasEpisodeInFilename(
        string fileName)
    {
        for (var i = 1; i < 50; i++)
        {
            var str = i.ToString();
            if (i < 10)
            {
                str = $"0{str}";
            }

            if (fileName.ContainsIgnoreCase($"E{str}"))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasSeasonInFilename(
        string fileName)
    {
        for (var i = 1; i < 50; i++)
        {
            var str = i.ToString();
            if (i < 10)
            {
                str = $"0{str}";
            }

            if (fileName.ContainsIgnoreCase($"S{str}"))
            {
                return true;
            }
        }

        return false;
    }

    private string GetSearchTitleForTorrentProviders(
        string title)
    {
        title = title.RemoveSpecialCharacters(" ");
        title = title.Replace("S H I E L D", "S.H.I.E.L.D");
        title = title.Replace("Marvel's", "");
        return title;
    }

    private bool MatchMediaTitleAndTorrentName(
        string searchTitle,
        string torrentName)
    {
        var searchTitleFormatted = searchTitle.RemoveSpecialCharacters(" ").RemoveDoubleSpacesAndTrim();
        var torrentNameFormatted = torrentName.RemoveSpecialCharacters(" ").RemoveDoubleSpacesAndTrim();
        var isMatch = torrentNameFormatted.ContainsIgnoreCase($"{searchTitleFormatted} ");
        return isMatch;
    }
}
