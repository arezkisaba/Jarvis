using Microsoft.AspNetCore.Components;

namespace Jarvis.Technical;

public abstract class BlazorLayoutComponentBase : LayoutComponentBase
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
