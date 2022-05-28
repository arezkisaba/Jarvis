using Jarvis.Technical;
using Microsoft.AspNetCore.Components;

namespace Jarvis.Shared.Components.Loader;

public partial class LoaderComponent : BlazorComponentBase
{
    [Parameter]
    public string AdditionalCssClass { get; set; }

    protected override void OnInitialized()
    {
    }
}
