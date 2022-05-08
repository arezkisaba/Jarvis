using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Jarvis.Pages.Home;

public partial class Home : BlazorPageComponentBase
{
    [Inject]
    private IIPResolverBackgroundAgent IPResolverBackgroundAgent { get; set; }

    [Inject]
    private IMediaStorageBackgroundAgent MediaStorageBackgroundAgent { get; set; }

    [Inject]
    private IGameControllerClientBackgroundAgent GameControllerClientBackgroundAgent { get; set; }

    [Inject]
    private IVPNClientBackgroundAgent VPNClientBackgroundAgent { get; set; }

    [Inject]
    private ITorrentClientBackgroundAgent TorrentClientBackgroundAgent { get; set; }

    private string PublicIPAddress { get; set; }

    private MediaStorageStateViewModel MediaStorageStateViewModel { get; set; }

    private GameControllerClientStateViewModel GameControllerClientStateViewModel { get; set; }

    private VPNClientStateViewModel VPNClientStateViewModel { get; set; }

    private TorrentClientStateViewModel TorrentClientStateViewModel { get; set; }
                                      
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
        MediaStorageStateViewModel = new MediaStorageStateViewModel(MediaStorageBackgroundAgent.CurrentState);
        MediaStorageBackgroundAgent.StateChanged += async (sender, __) =>
        {
            MediaStorageStateViewModel.Update((MediaStorageStateModel)sender);
            await UpdateUIAsync();
        };
        GameControllerClientStateViewModel = new GameControllerClientStateViewModel(GameControllerClientBackgroundAgent.CurrentState);
        GameControllerClientBackgroundAgent.StateChanged += async (sender, __) =>
        {
            GameControllerClientStateViewModel.Update((GameControllerClientStateModel)sender);
            await UpdateUIAsync();
        };
        VPNClientStateViewModel = new VPNClientStateViewModel(VPNClientBackgroundAgent.CurrentState);
        VPNClientBackgroundAgent.StateChanged += async (sender, __) =>
        {
            await IPResolverBackgroundAgent.UpdateCurrentStateAsync();
            VPNClientStateViewModel.Update((VPNClientStateModel)sender);
            await UpdateUIAsync();
        };
        TorrentClientStateViewModel = new TorrentClientStateViewModel(TorrentClientBackgroundAgent.CurrentState);
        TorrentClientBackgroundAgent.StateChanged += async (sender, __) =>
        {
            TorrentClientStateViewModel.Update((TorrentClientStateModel)sender);
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
