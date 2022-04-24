namespace Jarvis;

public abstract class ClientStateModelBase
{
    public string Title { get; private set; }

    public string Subtitle { get; private set; }

    public bool IsActive { get; private set; }

    public ClientStateModelBase(
        string title,
        string subtitle,
        bool isActive)
    {
        Title = title;
        Subtitle = subtitle;
        IsActive = isActive;
    }
}
