using Jarvis.Shared.Components.AlertDialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Jarvis.Pages.Configuration;

public partial class Configuration : BlazorPageComponentBase
{
    [Inject]
    public ISecureAppSettingsService SecureAppSettingsService { get; set; }

    public SecureAppSettingsViewModel SecureAppSettingsViewModel { get; set; }

    public EditContext EditContext { get; set; }

    public AlertDialog AlertDialogSuccessRef { get; set; }

    public AlertDialog AlertDialogErrorRef { get; set; }

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
            await SecureAppSettingsService.WriteAsync(SecureAppSettingsViewModel.ToDomain());
            AlertDialogSuccessRef.Show();
        }
        catch (Exception)
        {
            AlertDialogErrorRef.Show();
        }
    }
}
