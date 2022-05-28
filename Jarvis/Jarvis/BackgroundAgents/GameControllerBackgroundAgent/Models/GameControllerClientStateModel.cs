namespace Jarvis;

public class GameControllerClientStateModel : ClientStateModelBase
{
    public GameControllerClientStateModel(
        string title,
        string subtitle,
        bool isActive)
        : base(title, subtitle, isActive)
    {
    }
}
