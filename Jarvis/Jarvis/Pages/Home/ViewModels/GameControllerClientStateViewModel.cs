using Jarvis.Pages.Home;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Jarvis;

public sealed class GameControllerClientStateViewModel : ClientStateViewModelBase
{
    [Inject]
    public IStringLocalizer<Home> Localizer { get; set; }

    public GameControllerClientStateViewModel(
        GameControllerClientStateModel model)
        : base(model)
    {
    }
}
