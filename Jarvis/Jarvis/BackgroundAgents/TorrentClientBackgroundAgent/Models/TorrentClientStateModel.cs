namespace Jarvis;

public class TorrentClientStateModel : ClientStateModelBase
{
    public TorrentClientStateModel(
        string title,
        string subtitle,
        bool isActive)
        : base (title, subtitle, isActive)
    {
    }
}
