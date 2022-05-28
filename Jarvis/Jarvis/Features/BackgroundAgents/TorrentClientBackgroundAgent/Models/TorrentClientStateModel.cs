namespace Jarvis.Features.BackgroundAgents.TorrentClientBackgroundAgent.Models;

public class TorrentClientStateModel
{
    public string Title { get; private set; }

    public string Subtitle { get; private set; }

    public bool IsActive { get; private set; }

    public TorrentClientStateModel(
        string title,
        string subtitle,
        bool isActive)
    {
        Title = title;
        Subtitle = subtitle;
        IsActive = isActive;
    }
}
