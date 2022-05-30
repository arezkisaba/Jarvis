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

namespace Jarvis.Pages.Home;

public partial class Home : BlazorPageComponentBase
{
    [Inject]
    private IIPResolverAgent IPResolverBackgroundAgent { get; set; }
    
    [Inject]
    private IMediaStorageAgent MediaStorageBackgroundAgent { get; set; }
    
    [Inject]
    private IGameControllerClientAgent GameControllerClientBackgroundAgent { get; set; }
    
    [Inject]
    private IVPNClientAgent VPNClientBackgroundAgent { get; set; }
    
    [Inject]
    private ITorrentClientAgent TorrentClientBackgroundAgent { get; set; }
    
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
        PublicIPAddress = IPResolverBackgroundAgent.CurrentState;
        IPResolverBackgroundAgent.StateChanged += async (sender, __) =>
        {
            PublicIPAddress = (string)sender;
            await UpdateUIAsync();
        };
        MediaStorageStateViewModel = new MediaStorageStateViewModel(MediaSizingService, MediaStorageBackgroundAgent.CurrentState);
        MediaStorageBackgroundAgent.StateChanged += async (sender, __) =>
        {
            MediaStorageStateViewModel.Update((MediaStorageStateModel)sender);
            await UpdateUIAsync();
        };
        GameControllerClientStateViewModel = new GameControllerClientStateViewModel(GameControllerClientBackgroundAgent.CurrentState);
        GameControllerClientBackgroundAgent.StateChanged += async (sender, __) =>
        {
            var model = (GameControllerClientStateModel)sender;
            GameControllerClientStateViewModel.UpdateInternalData(model);
            await UpdateUIAsync();
        };
        VPNClientStateViewModel = new VPNClientStateViewModel(VPNClientBackgroundAgent.CurrentState);
        VPNClientBackgroundAgent.RefreshIsClientActive();
        VPNClientBackgroundAgent.StateChanged += async (sender, __) =>
        {
            await IPResolverBackgroundAgent.UpdateCurrentStateAsync();

            var model = (VPNClientStateModel)sender;
            VPNClientStateViewModel.UpdateInternalData((VPNClientStateModel)sender);
            await UpdateUIAsync();
        };

        TorrentClientStateViewModel = new TorrentClientStateViewModel(TorrentClientBackgroundAgent.CurrentState);
        TorrentClientBackgroundAgent.RefreshIsClientActive();
        TorrentClientBackgroundAgent.StateChanged += async (sender, __) =>
        {
            var model = (TorrentClientStateModel)sender;
            TorrentClientStateViewModel.UpdateInternalData((TorrentClientStateModel)sender);
            await UpdateUIAsync();
        };
    }

    private async Task ToggleVPNClientStatusAsync(
        MouseEventArgs e)
    {
        if (VPNClientBackgroundAgent.CurrentState.IsActive)
        {
            await VPNClientBackgroundAgent.StopClientAsync();
        }
        else
        {
            await VPNClientBackgroundAgent.StartClientAsync();
        }
    }

    private async Task ToggleTorrentClientStatusAsync(
        MouseEventArgs e)
    {
        if (TorrentClientBackgroundAgent.CurrentState.IsActive)
        {
            await TorrentClientBackgroundAgent.StopClientAsync();
        }
        else
        {
            await TorrentClientBackgroundAgent.StartClientAsync();
        }
    }
}
