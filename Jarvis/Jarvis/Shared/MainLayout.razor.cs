using Lib.ApiServices.Tmdb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Jarvis.Shared;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IOptions<AppSettings> AppSettings { get; set; }

    [Inject]
    public ISecureAppSettingsService SecureAppSettingsService { get; set; }

    [Inject]
    public ITmdbApiService TmdbApiService { get; set; }

    public string AppName { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        AppName = AppSettings.Value.appName;

        SetCultureInfo();

        TmdbApiService.AuthenticationInformationsAvailable += (sender, e) =>
        {
            // Popup
        };

        TmdbApiService.AuthenticationSuccessfull += async (sender, e) =>
        {
            var secureAppSettings = await SecureAppSettingsService.ReadAsync();
            secureAppSettings.TmdbSessionId = e.SessionId;
            await SecureAppSettingsService.WriteAsync(secureAppSettings);
        };
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
