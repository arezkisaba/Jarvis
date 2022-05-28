using Microsoft.AspNetCore.Components;

namespace Jarvis.Shared.Components.Modal;

public partial class Modal : BlazorComponentBase
{
    [Parameter]
    public string AdditionalCssClass { get; set; }

    [Parameter]
    public RenderFragment Title { get; set; }

    [Parameter]
    public RenderFragment Body { get; set; }

    [Parameter]
    public bool Show { get; set; }
}
