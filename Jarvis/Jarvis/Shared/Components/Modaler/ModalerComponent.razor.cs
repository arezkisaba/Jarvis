using Jarvis.Shared.Components.Modaler.Services;
using Microsoft.AspNetCore.Components;

namespace Jarvis.Shared.Components.Modaler;

public partial class ModalerComponent : ComponentBase, IDisposable
{
    [Inject]
    private ModalerService _modalerService { get; set; }

    private ModalerService modalerService => _modalerService!;

    protected override void OnInitialized()
    {
        modalerService.ModalerChanged += OnModalChanged;
    }

    private void OnModalChanged(
        object sender,
        EventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        modalerService.ModalerChanged -= OnModalChanged;
    }
}