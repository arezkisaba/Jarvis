using Jarvis.Shared.Components.Toaster;
using Lib.ApiServices.Tmdb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Jarvis.Shared;

public partial class MainLayout : BlazorLayoutComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public ToasterService ToasterService { get; set; }

    [Inject]
    public IOptions<AppSettings> AppSettings { get; set; }

    [Inject]
    public ISecureAppSettingsService SecureAppSettingsService { get; set; }

    [Inject]
    public ITmdbApiService TmdbApiService { get; set; }

    public bool ShowTmdbModal { get; set; }

    public bool ShowAlert { get; set; }

    public string TmdbAuthenticationUrl { get; set; }

    public string AppName { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        AppName = AppSettings.Value.appName;

        SetCultureInfo();

        TmdbApiService.AuthenticationInformationsAvailable += async (sender, e) =>
        {
            TmdbAuthenticationUrl = e.AuthenticationUrl;
            ShowTmdbModal = true;
            await UpdateUIAsync();
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
                TmdbAuthenticationUrl = null;
                ShowTmdbModal = false;
                await UpdateUIAsync();
            }
        };

        ToasterService.AddToast(Toast.CreateToast("Hello World", "Hello from Blazor", ToastType.Info, 600));

        Task.Run(async () =>
        {
            ////await Task.Delay(2000);
            ////ShowAlert = true;
            ////await UpdateUIAsync();
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
