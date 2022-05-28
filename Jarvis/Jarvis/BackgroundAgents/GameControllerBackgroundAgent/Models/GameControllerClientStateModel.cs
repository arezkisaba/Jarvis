namespace Jarvis.BackgroundAgents.GameControllerBackgroundAgent.Models;

public class GameControllerClientStateModel
{
    public string Title { get; private set; }

    public string Subtitle { get; private set; }

    public bool IsActive { get; private set; }

    public GameControllerClientStateModel(
        string title,
        string subtitle,
        bool isActive)
    {
        Title = title;
        Subtitle = subtitle;
        IsActive = isActive;
    }
}
