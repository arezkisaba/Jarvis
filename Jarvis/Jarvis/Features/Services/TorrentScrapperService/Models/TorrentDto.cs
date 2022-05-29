namespace Jarvis.Features.Services.TorrentScrapperService.Models;

public readonly record struct TorrentDto(
    string DescriptionUrl,
    string Name,
    string Provider,
    int Seeds,
    long Size);
