using Microsoft.AspNetCore.Components;

namespace Jarvis.Shared.Components.Toaster;

public partial class Toaster : ComponentBase, IDisposable
{
    [Inject]
    private ToasterService _toasterService { get; set; }

    private ToasterService toasterService => _toasterService!;

    protected override void OnInitialized()
    {
        toasterService.ToasterChanged += ToastChanged;
        toasterService.ToasterTimerElapsed += ToastChanged;
    }

    private void ClearToast(Toast toast)
    {
        toasterService.ClearToast(toast);
    }

    private void ToastChanged(object? sender, EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        toasterService.ToasterChanged -= ToastChanged;
        toasterService.ToasterTimerElapsed -= ToastChanged;
    }
}