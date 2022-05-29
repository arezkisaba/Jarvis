using Jarvis.Features.Agents.TorrentClientAgent.Models;

namespace Jarvis.Pages.Home.ViewModels;

public sealed class TorrentClientStateViewModel
{
    public string Title { get; private set; }

    public string Subtitle { get; private set; }

    public bool IsActive { get; private set; }

    public TorrentClientStateViewModel(
        TorrentClientStateModel model)
    {
        UpdateInternalData(model);
    }

    public void UpdateInternalData(
        TorrentClientStateModel model)
    {
        Title = model.Title;
        Subtitle = model.Subtitle;
        IsActive = model.IsActive;
    }
}
