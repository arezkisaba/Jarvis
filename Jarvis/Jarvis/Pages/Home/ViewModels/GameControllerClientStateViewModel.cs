using Jarvis.BackgroundAgents.GameControllerBackgroundAgent.Models;

namespace Jarvis.Pages.Home.ViewModels;

public sealed class GameControllerClientStateViewModel
{
    public string Title { get; private set; }

    public string Subtitle { get; private set; }

    public bool IsActive { get; private set; }

    public GameControllerClientStateViewModel(
        GameControllerClientStateModel model)
    {
        UpdateInternalData(model);
    }

    public void UpdateInternalData(
        GameControllerClientStateModel model)
    {
        Title = model.Title;
        Subtitle = model.Subtitle;
        IsActive = model.IsActive;
    }
}
