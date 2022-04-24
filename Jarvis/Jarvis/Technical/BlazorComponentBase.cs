using Microsoft.AspNetCore.Components;

namespace Jarvis;

public abstract class BlazorComponentBase : ComponentBase
{
    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    protected Task UpdateUIAsync()
    {
        return InvokeAsync(StateHasChanged);
    }
}
