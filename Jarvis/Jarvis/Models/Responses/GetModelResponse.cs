namespace Jarvis;

public class GetModelResponse
{
    public string AppName { get; set; }
    public bool isXboxServiceClientActive { get; set; }
    public string isXboxServiceClientActiveSubtitle { get; set; }
    public bool isVpnServiceClientActive { get; set; }
    public string isVpnServiceClientActiveSubtitle { get; set; }
    public bool isTorrentServiceClientActive { get; set; }
    public string isTorrentServiceClientActiveSubtitle { get; set; }
    public IEnumerable<LogEntry> logEntries { get; set; }
    public HomeSection Home { get; set; }
    public DownloadsSection Downloads { get; set; }

    public class LogEntry
    {
        public LogLevel level { get; set; }
        public string text { get; set; }
    }

    public class HomeSection
    {
        public string displayName { get; set; }
        public long availableSpaceSize { get; set; }
        public long usedSpaceSize { get; set; }
        public long totalSpaceSize { get; set; }
        public double moviesFolderSize { get; set; }
        public double tvShowsFolderSize { get; set; }
    }

    public class DownloadsSection
    {
        public string displayName { get; set; }
        public IEnumerable<DownloadsSectionItem> downloads { get; set; }
    }

    public class DownloadsSectionItem
    {
        public int id { get; set; }
        public string hashString { get; set; }
        public string torrentName { get; set; }
        public string torrentProvider { get; set; }
        public double? percentDone { get; set; }
        public DownloadMode downloadMode { get; set; }
        public string torrentUrl { get; set; }
        public string downloadDirectory { get; set; }
        public string language { get; set; }
        public bool hasBeenCopied { get; set; }
        public long seeders { get; set; }
        public double rateDownload { get; internal set; }
        public double rateUpload { get; internal set; }
        public double sizeWhenDone { get; internal set; }
        public double leftUntilDone { get; internal set; }
        public int peersConnected { get; internal set; }
        public int peersGettingFromUs { get; internal set; }
        public int peersSendingToUs { get; internal set; }
        public string movieStorageTitle { get; set; }
        public int? movieYear { get; set; }
        public string tvShowStorageTitle { get; set; }
        public int seasonNumber { get; set; }
        public int seasonEpisodesCount { get; set; }
        public int episodeNumber { get; set; }

        private DownloadsSectionItem(
            DownloadMode downloadMode,
            string torrentName,
            string torrentProvider,
            string torrentUrl,
            string downloadDirectory,
            string language,
            long seeders,
            double sizeWhenDone)
        {
            this.downloadMode = downloadMode;
            this.torrentName = torrentName;
            this.torrentProvider = torrentProvider;
            this.torrentUrl = torrentUrl;
            this.downloadDirectory = Path.Combine(downloadDirectory, torrentName);
            this.language = language;
            this.seeders = seeders;
            this.sizeWhenDone = sizeWhenDone;
        }

        public DownloadsSectionItem(
            DownloadMode downloadMode,
            string torrentName,
            string torrentProvider,
            string torrentUrl,
            string downloadDirectory,
            string language,
            long seeders,
            double sizeWhenDone,
            string movieStorageTitle,
            int movieYear)
            : this(downloadMode, torrentName, torrentProvider, torrentUrl, downloadDirectory, language, seeders, sizeWhenDone)
        {
            this.movieStorageTitle = movieStorageTitle;
            this.movieYear = movieYear;
        }

        public DownloadsSectionItem(
            DownloadMode downloadMode,
            string torrentName,
            string torrentProvider,
            string torrentUrl,
            string downloadDirectory,
            string language,
            long seeders,
            double sizeWhenDone,
            string tvShowStorageTitle,
            int seasonNumber,
            int seasonEpisodesCount)
            : this(downloadMode, torrentName, torrentProvider, torrentUrl, downloadDirectory, language, seeders, sizeWhenDone)
        {
            this.tvShowStorageTitle = tvShowStorageTitle;
            this.seasonNumber = seasonNumber;
            this.seasonEpisodesCount = seasonEpisodesCount;
        }

        public DownloadsSectionItem(
            DownloadMode downloadMode,
            string torrentName,
            string torrentProvider,
            string torrentUrl,
            string downloadDirectory,
            string language,
            long seeders,
            double sizeWhenDone,
            string tvShowStorageTitle,
            int seasonNumber,
            int seasonEpisodesCount,
            int episodeNumber)
            : this(downloadMode, torrentName, torrentProvider, torrentUrl, downloadDirectory, language, seeders, sizeWhenDone, tvShowStorageTitle, seasonNumber, seasonEpisodesCount)
        {
            this.episodeNumber = episodeNumber;
        }

        public string GetDisplayTitle()
        {
            if (downloadMode == DownloadMode.Movie)
            {
                return movieStorageTitle;
            }
            else if (downloadMode == DownloadMode.Season)
            {
                return $"{tvShowStorageTitle} {FormatHelper.GetSeasonShortName(seasonNumber)}";
            }
            else if (downloadMode == DownloadMode.Episode)
            {
                return $"{tvShowStorageTitle} {FormatHelper.GetSeasonShortName(seasonNumber)}{FormatHelper.GetEpisodeShortName(episodeNumber)}";
            }
            else
            {
                throw new InvalidOperationException($"DownloadMode {downloadMode} unknown");
            }
        }
    }
}
