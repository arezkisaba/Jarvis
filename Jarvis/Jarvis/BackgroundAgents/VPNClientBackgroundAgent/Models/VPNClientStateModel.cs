namespace Jarvis.BackgroundAgents.VPNClientBackgroundAgent.Models;

public class VPNClientStateModel
{
    public string Title { get; private set; }

    public string Subtitle { get; private set; }

    public bool IsActive { get; private set; }

    public VPNClientStateModel(
        string title,
        string subtitle,
        bool isActive)
    {
        Title = title;
        Subtitle = subtitle;
        IsActive = isActive;
    }
}
