namespace Jarvis;

public readonly record struct TorrentDto(
    string DescriptionUrl,
    string Name,
    string Provider,
    int Seeds,
    double Size);
