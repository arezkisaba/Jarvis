using Microsoft.Extensions.Options;
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
        InitAsync().Wait();
    }

    private async Task InitAsync()
    {
        SecureAppSettings encryptedSecrets;

        ////var tempConfig = builder.Build();
        ////var folderPath = tempConfig.GetValue<string>($"AppSettings:{nameof(AppSettings.appdataDirectory)}");
        ////if (!Directory.Exists(folderPath))
        ////{
        ////    Directory.CreateDirectory(folderPath);
        ////}

        ////var filePath = $"{Path.Combine(folderPath, "Secrets.json")}";
        ////if (!File.Exists(filePath))
        ////{
        ////    File.Create(filePath);
        ////}

        try
        {
            encryptedSecrets = await _secureAppSettingsService.ReadAsync();
        }
        catch (Exception)
        {
            encryptedSecrets = new SecureAppSettings();
            await _secureAppSettingsService.WriteAsync(encryptedSecrets);
        }

        Data = encryptedSecrets.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(prop => $"{nameof(SecureAppSettings)}:{prop.Name}", prop => (string)prop.GetValue(encryptedSecrets, null));
    }
}
