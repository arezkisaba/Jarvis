using Jarvis.Technical.Configuration.SecureAppSettings.Models;
using Jarvis.Technical.Configuration.SecureAppSettings.Services.Contracts;

namespace Jarvis.Technical.Configuration.SecureAppSettings;

public class SecureAppSettingsConfigurationProvider : ConfigurationProvider
{
    private readonly ISecureAppSettingsService _secureAppSettingsService;

    public SecureAppSettingsConfigurationProvider(
        ISecureAppSettingsService secureAppSettingsService)
    {
        _secureAppSettingsService = secureAppSettingsService;
    }

    public override void Load()
    {
        var encryptedSecrets = CreateSecureAppSettingsIfDoesntExistsAsync().GetAwaiter().GetResult();
        Data = encryptedSecrets.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(prop => $"SecureAppSettings:{prop.Name}", prop => (string)prop.GetValue(encryptedSecrets, null));
    }

    #region Private use

    private async Task<SecureAppSettingsModel> CreateSecureAppSettingsIfDoesntExistsAsync()
    {
        SecureAppSettingsModel encryptedSecrets;

        try
        {
            encryptedSecrets = await _secureAppSettingsService.ReadAsync();
        }
        catch (Exception)
        {
            encryptedSecrets = new SecureAppSettingsModel();
            await _secureAppSettingsService.WriteAsync(encryptedSecrets);
        }

        return encryptedSecrets;
    }

    #endregion
}
