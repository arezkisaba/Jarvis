using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace Jarvis.Pages.Configuration;

public partial class Configuration : BlazorPageComponentBase
{
    [Inject]
    public IOptions<SecureAppSettings> SecureAppSettings { get; set; }

    [Inject]
    public ISecureAppSettingsService SecureAppSettingsService { get; set; }

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
        try
        {
            var domain = SecureAppSettingsViewModel.ToDomain();
            domain.TmdbSessionId = SecureAppSettings.Value.TmdbSessionId;
            await SecureAppSettingsService.WriteAsync(domain);
            ////AlertDialogSuccessRef.Show();
        }
        catch (Exception)
        {
            ////AlertDialogErrorRef.Show();
        }
    }
}
