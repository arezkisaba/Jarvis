using Jarvis.Pages.Home;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Jarvis;

public sealed class VPNClientStateViewModel : ClientStateViewModelBase
{
    [Inject]
    public IStringLocalizer<Home> Localizer { get; set; }

    public VPNClientStateViewModel(
        VPNClientStateModel model)
        : base(model)
    {
    }
}
