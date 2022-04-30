using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Jarvis.Pages.Configuration;

public partial class Configuration : BlazorPageComponentBase
{
    [Inject]
    public ISecureAppSettingsService SecureAppSettingsService { get; set; }

    [Inject]
    public IVPNClientBackgroundAgent VPNClientBackgroundAgent { get; set; }

    public SecureAppSettingsViewModel SecureAppSettingsViewModel { get; set; }

    public EditContext EditContext { get; set; }

    protected override async Task OnInitializedAsync()
    {
        PageTitle = "Configuration";
        var secureAppSettings = await SecureAppSettingsService.ReadAsync();
        SecureAppSettingsViewModel = new SecureAppSettingsViewModel(secureAppSettings);
        EditContext = new(SecureAppSettingsViewModel);
    }

    public void Dispose()
    {
        // Unsuscribe
    }

    public async Task HandleOnValidSubmitAsync()
    {
        await SecureAppSettingsService.WriteAsync(SecureAppSettingsViewModel.ToDomain());
    }
}
