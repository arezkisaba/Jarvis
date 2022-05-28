using Jarvis.Shared.Components.Toaster.Models;
using Jarvis.Shared.Components.Toaster.Services;
using Microsoft.AspNetCore.Components;

namespace Jarvis.Shared.Components.Toaster;

public partial class ToasterComponent : ComponentBase, IDisposable
{
    [Inject]
    private ToasterService _toasterService { get; set; }

    private ToasterService toasterService => _toasterService!;

    protected override void OnInitialized()
    {
        toasterService.ToasterChanged += OnToastChanged;
        toasterService.ToasterTimerElapsed += OnToastChanged;
    }

    private void ClearToast(
        Toast toast)
    {
        toasterService.ClearToast(toast);
    }

    private void OnToastChanged(
        object sender,
        EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        toasterService.ToasterChanged -= OnToastChanged;
        toasterService.ToasterTimerElapsed -= OnToastChanged;
    }
}