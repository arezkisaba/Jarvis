using Jarvis.Shared.Components.Modaler.Services;
using Jarvis.Shared.Components.Toaster.Services;
using Jarvis.Technical;
using Jarvis.Technical.Configuration.AppSettings.Models;
using Jarvis.Technical.Configuration.SecureAppSettings.Services.Contracts;
using Lib.ApiServices.Tmdb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;

namespace Jarvis.Shared;

public partial class MainLayout : BlazorLayoutComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Inject]
    private ModalerService ModalerService { get; set; }

    [Inject]
    private ToasterService ToasterService { get; set; }

    [Inject]
    private IOptions<AppSettingsModel> AppSettings { get; set; }

    [Inject]
    private ISecureAppSettingsService SecureAppSettingsService { get; set; }

    [Inject]
    private ITmdbApiService TmdbApiService { get; set; }

    private bool ShowAlert { get; set; }

    private string TmdbAuthenticationUrl { get; set; }

    private string AppName { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        AppName = AppSettings.Value.appName;

        SetCultureInfo();

        TmdbApiService.AuthenticationInformationsAvailable += async (sender, e) =>
        {
            TmdbAuthenticationUrl = e.AuthenticationUrl;
            ////await UpdateUIAsync();
        };

        TmdbApiService.AuthenticationSuccessfull += async (sender, e) =>
        {
            try
            {
                var secureAppSettings = await SecureAppSettingsService.ReadAsync();
                secureAppSettings.TmdbSessionId = e.SessionId;
                await SecureAppSettingsService.WriteAsync(secureAppSettings);
            }
            catch (Exception)
            {
                // IGNORE
            }
            finally
            {
            }
        };

        Task.Run(async () =>
        {
            ////await Task.Delay(2000);
            ////ToasterService.AddToast(Toast.CreateToast("Hello World 1", "Hello from Blazor", ToastType.Success, 5));
            ////await Task.Delay(2000);
            ////ToasterService.AddToast(Toast.CreateToast("Hello World 2", "Hello from Blazor", ToastType.Danger, 5));
            ////await Task.Delay(2000);
            ////ModalerService.AddModal(Modal.CreateModal("Hello World 1", "Hello from Blazor", ModalType.Warning));
        });
    }

    #region Private use

    private void SetCultureInfo()
    {
        if (QueryHelpers.ParseQuery(new Uri(NavigationManager.Uri).Query).TryGetValue("lang", out var paramLang))
        {
            CultureInfo culture;

            try
            {
                culture = new CultureInfo(paramLang);
            }
            catch
            {
                culture = new CultureInfo("en");
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }

    #endregion
}
