using System.Reflection;

namespace Jarvis;

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
            .ToDictionary(prop => $"{nameof(SecureAppSettings)}:{prop.Name}", prop => (string)prop.GetValue(encryptedSecrets, null));
    }

    #region Private use

    private async Task<SecureAppSettings> CreateSecureAppSettingsIfDoesntExistsAsync()
    {
        SecureAppSettings encryptedSecrets;

        try
        {
            encryptedSecrets = await _secureAppSettingsService.ReadAsync();
        }
        catch (Exception)
        {
            encryptedSecrets = new SecureAppSettings();
            await _secureAppSettingsService.WriteAsync(encryptedSecrets);
        }

        return encryptedSecrets;
    }

    #endregion
}
