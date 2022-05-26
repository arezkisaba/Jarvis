using Microsoft.AspNetCore.Components;

namespace Jarvis.Shared.Components.StretchContainer;

public partial class StretchContainer : BlazorComponentBase
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    protected override void OnInitialized()
    {
    }
}
