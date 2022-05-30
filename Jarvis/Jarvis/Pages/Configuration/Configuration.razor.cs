using Jarvis.Pages.Configuration.ViewModels;
using Jarvis.Shared.Components.Toaster.Models;
using Jarvis.Shared.Components.Toaster.Services;
using Jarvis.Technical;
using Jarvis.Technical.Configuration.SecureAppSettings.Models;
using Jarvis.Technical.Configuration.SecureAppSettings.Services.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

namespace Jarvis.Pages.Configuration;

public partial class Configuration : BlazorPageComponentBase
{
    [Inject]
    private IOptions<SecureAppSettingsModel> SecureAppSettings { get; set; }
    
    [Inject]
    private IStringLocalizer<App> AppLocalizer { get; set; }
    
    [Inject]
    private IStringLocalizer<Configuration> Localizer { get; set; }
    
    [Inject]
    private ISecureAppSettingsService SecureAppSettingsService { get; set; }
    
    [Inject]
    private ToasterService ToasterService { get; set; }

    private SecureAppSettingsViewModel SecureAppSettingsViewModel { get; set; }

    private EditContext EditContext { get; set; }

    protected override async Task OnInitializedAsync()
    {
        PageTitle = "Configuration";
        var secureAppSettings = await SecureAppSettingsService.ReadAsync();
        SecureAppSettingsViewModel = new SecureAppSettingsViewModel(secureAppSettings);
        EditContext = new(SecureAppSettingsViewModel);
    }

    private async Task HandleOnValidSubmitAsync()
    {
        try
        {
            var domain = SecureAppSettingsViewModel.ToDomain();
            domain.TmdbSessionId = SecureAppSettings.Value.TmdbSessionId;
            await SecureAppSettingsService.WriteAsync(domain);

            ToasterService.AddToast(Toast.CreateToast(AppLocalizer["Toaster.InformationTitle"], Localizer["Toaster.ConfigurationUpdated"], ToastType.Success, 2));
        }
        catch (Exception)
        {
            ToasterService.AddToast(Toast.CreateToast(AppLocalizer["Toaster.ErrorTitle"], AppLocalizer["Toaster.ErrorMessage"], ToastType.Danger, 2));
        }
    }
}
