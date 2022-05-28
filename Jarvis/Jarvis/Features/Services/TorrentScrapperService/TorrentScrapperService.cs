using Jarvis.Features.Services.TorrentScrapperService.Contracts;
using Jarvis.Features.Services.TorrentScrapperService.Models;
using Jarvis.Features.Services.TorrentScrapperService.Providers.Bases;

namespace Jarvis.Features.Services.TorrentScrapperService;

public class TorrentScrapperService : ITorrentScrapperService
{
    private readonly IEnumerable<TorrentScrapperServiceBase> _torrentScrapperServices;

    public TorrentScrapperService(
        IEnumerable<TorrentScrapperServiceBase> torrentScrapperServices)
    {
        _torrentScrapperServices = torrentScrapperServices;
    }

    public async Task<List<TorrentDto>> GetResultsAsync(
        string query)
    {
        var torrents = new List<TorrentDto>();
        var tasks = new List<Task<List<TorrentDto>>>();

        foreach (var _torrentScrapperService in _torrentScrapperServices)
        {
            try
            {
                var task = _torrentScrapperService.GetResultsAsync(query);
                tasks.Add(task);
            }
            catch (Exception)
            {
                // IGNORE
            }
        }

        var allTorrents = await Task.WhenAll(tasks);
        foreach (var torrentsFromProvider in allTorrents)
        {
            torrents.AddRange(torrentsFromProvider);
        }

        return torrents;
    }

    public Task<string> GetDownloadLinkAsync(
        string provider,
        string descriptionUrl,
        string cookies,
        string userAgent)
    {
        return _torrentScrapperServices.Single(obj => obj.Name == provider).GetDownloadLinkAsync(descriptionUrl, cookies, userAgent);
    }
}
