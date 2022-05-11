using Jarvis.Pages.Home;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Jarvis;

public sealed class TorrentClientStateViewModel : ClientStateViewModelBase
{
    [Inject]
    public IStringLocalizer<Home> Localizer { get; set; }

    public TorrentClientStateViewModel(
        TorrentClientStateModel model)
        : base(model)
    {
    }
}
