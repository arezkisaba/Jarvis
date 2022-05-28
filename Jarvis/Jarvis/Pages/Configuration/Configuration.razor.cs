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
    public IOptions<SecureAppSettingsModel> SecureAppSettings { get; set; }

    [Inject]
    public IStringLocalizer<App> AppLoc { get; set; }

    [Inject]
    public IStringLocalizer<Configuration> Loc { get; set; }

    [Inject]
    public ISecureAppSettingsService SecureAppSettingsService { get; set; }

    public SecureAppSettingsViewModel SecureAppSettingsViewModel { get; set; }

    [Inject]
    public ToasterService ToasterService { get; set; }

    public EditContext EditContext { get; set; }

    protected override async Task OnInitializedAsync()
    {
        PageTitle = "Configuration";
        var secureAppSettings = await SecureAppSettingsService.ReadAsync();
        SecureAppSettingsViewModel = new SecureAppSettingsViewModel(secureAppSettings);
        EditContext = new(SecureAppSettingsViewModel);
    }

    public async Task HandleOnValidSubmitAsync()
    {
        try
        {
            var domain = SecureAppSettingsViewModel.ToDomain();
            domain.TmdbSessionId = SecureAppSettings.Value.TmdbSessionId;
            await SecureAppSettingsService.WriteAsync(domain);

            ToasterService.AddToast(Toast.CreateToast(AppLoc["Toaster.InformationTitle"], Loc["Toaster.ConfigurationUpdated"], ToastType.Success, 2));
        }
        catch (Exception)
        {
            ToasterService.AddToast(Toast.CreateToast(AppLoc["Toaster.ErrorTitle"], AppLoc["Toaster.ErrorMessage"], ToastType.Danger, 2));
        }
    }
}
