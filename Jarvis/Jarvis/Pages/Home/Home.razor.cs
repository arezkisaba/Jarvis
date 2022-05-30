using Jarvis.Features.Agents.GameControllerAgent.Contracts;
using Jarvis.Features.Agents.GameControllerAgent.Models;
using Jarvis.Features.Agents.IPResolverAgent.Contracts;
using Jarvis.Features.Agents.MediaStorageAgent.Contracts;
using Jarvis.Features.Agents.MediaStorageAgent.Models;
using Jarvis.Features.Agents.TorrentClientAgent.Contracts;
using Jarvis.Features.Agents.TorrentClientAgent.Models;
using Jarvis.Features.Agents.VPNClientAgent.Contracts;
using Jarvis.Features.Agents.VPNClientAgent.Models;
using Jarvis.Features.Services.MediaSizingService.Contracts;
using Jarvis.Pages.Home.ViewModels;
using Jarvis.Technical;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;

namespace Jarvis.Pages.Home;

public partial class Home : BlazorPageComponentBase
{
    [Inject]
    private IStringLocalizer<Home> Localizer { get; set; }

    [Inject]
    private IIPResolverAgent IPResolverAgent { get; set; }
    
    [Inject]
    private IMediaStorageAgent MediaStorageAgent { get; set; }
    
    [Inject]
    private IGameControllerClientAgent GameControllerClientAgent { get; set; }
    
    [Inject]
    private IVPNClientAgent VPNClientAgent { get; set; }
    
    [Inject]
    private ITorrentClientAgent TorrentClientAgent { get; set; }
    
    [Inject]
    private IMediaSizingService MediaSizingService { get; set; }

    private MediaStorageStateViewModel MediaStorageStateViewModel { get; set; }

    private GameControllerClientStateViewModel GameControllerClientStateViewModel { get; set; }

    private VPNClientStateViewModel VPNClientStateViewModel { get; set; }

    private TorrentClientStateViewModel TorrentClientStateViewModel { get; set; }

    private string PublicIPAddress { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        PageTitle = "Home";
        PublicIPAddress = IPResolverAgent.CurrentState;
        IPResolverAgent.StateChanged += async (sender, __) =>
        {
            PublicIPAddress = (string)sender;
            await UpdateUIAsync();
        };
        MediaStorageStateViewModel = new MediaStorageStateViewModel(MediaSizingService, MediaStorageAgent.CurrentState);
        MediaStorageAgent.StateChanged += async (sender, __) =>
        {
            MediaStorageStateViewModel.Update((MediaStorageStateModel)sender);
            await UpdateUIAsync();
        };
        GameControllerClientStateViewModel = new GameControllerClientStateViewModel(GameControllerClientAgent.CurrentState);
        GameControllerClientAgent.StateChanged += async (sender, __) =>
        {
            var model = (GameControllerClientStateModel)sender;
            GameControllerClientStateViewModel.UpdateInternalData(model);
            await UpdateUIAsync();
        };
        VPNClientStateViewModel = new VPNClientStateViewModel(VPNClientAgent.CurrentState);
        VPNClientAgent.RefreshIsClientActive();
        VPNClientAgent.StateChanged += async (sender, __) =>
        {
            await IPResolverAgent.UpdateCurrentStateAsync();

            var model = (VPNClientStateModel)sender;
            VPNClientStateViewModel.UpdateInternalData((VPNClientStateModel)sender);
            await UpdateUIAsync();
        };

        TorrentClientStateViewModel = new TorrentClientStateViewModel(TorrentClientAgent.CurrentState);
        TorrentClientAgent.RefreshIsClientActive();
        TorrentClientAgent.StateChanged += async (sender, __) =>
        {
            var model = (TorrentClientStateModel)sender;
            TorrentClientStateViewModel.UpdateInternalData((TorrentClientStateModel)sender);
            await UpdateUIAsync();
        };
    }

    private async Task ToggleVPNClientStatusAsync(
        MouseEventArgs e)
    {
        if (VPNClientAgent.CurrentState.IsActive)
        {
            await VPNClientAgent.StopClientAsync();
        }
        else
        {
            await VPNClientAgent.StartClientAsync();
        }
    }

    private async Task ToggleTorrentClientStatusAsync(
        MouseEventArgs e)
    {
        if (TorrentClientAgent.CurrentState.IsActive)
        {
            await TorrentClientAgent.StopClientAsync();
        }
        else
        {
            await TorrentClientAgent.StartClientAsync();
        }
    }
}
