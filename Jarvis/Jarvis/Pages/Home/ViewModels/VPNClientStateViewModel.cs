using Jarvis.BackgroundAgents.VPNClientBackgroundAgent.Models;

namespace Jarvis.Pages.Home.ViewModels;

public sealed class VPNClientStateViewModel
{
    public string Title { get; private set; }

    public string Subtitle { get; private set; }

    public bool IsActive { get; private set; }

    public VPNClientStateViewModel(
        VPNClientStateModel model)
    {
        UpdateInternalData(model);
    }

    public void UpdateInternalData(
        VPNClientStateModel model)
    {
        Title = model.Title;
        Subtitle = model.Subtitle;
        IsActive = model.IsActive;
    }
}
