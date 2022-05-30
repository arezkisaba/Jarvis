using Jarvis.Technical;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Jarvis.Pages.Home.Components;

public partial class ServiceStateComponent : BlazorComponentBase
{
    [Parameter]
    public bool IsActive { get; set; }

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string Subtitle { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnStatusChangeRequestedCallback { get; set; }

    private bool HasPendingAction { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    private async void OnStatusChangeRequested()
    {
        HasPendingAction = true;

        try
        {
            await OnStatusChangeRequestedCallback.InvokeAsync();
        }
        catch (Exception)
        {
        }
        finally
        {
            HasPendingAction = false;
        }
    }
}
