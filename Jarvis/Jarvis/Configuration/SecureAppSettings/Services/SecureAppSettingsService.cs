using Lib.Core;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Jarvis;

public class SecureAppSettingsService : ISecureAppSettingsService
{
    private const string fileName = $"{nameof(SecureAppSettings)}.json";

    private readonly IDataProtector _dataProtector;
    private readonly string _filePath;

    public SecureAppSettingsService(
        string filePath)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDataProtection();
        var services = serviceCollection.BuildServiceProvider();
        var dataProtectionProvider = services.GetService<IDataProtectionProvider>();
        _dataProtector = dataProtectionProvider.CreateProtector($"{Assembly.GetEntryAssembly().GetName().Name}");
        _filePath = filePath;
    }

    public async Task<SecureAppSettings> ReadAsync()
    {
        var content = File.ReadAllText(_filePath);
        var encryptedSecrets = await Serializer.JsonDeserializeAsync<SecureAppSettings>(content);
        return new SecureAppSettings(
            Unprotect(encryptedSecrets.OpenVPNUsername),
            Unprotect(encryptedSecrets.OpenVPNPassword),
            Unprotect(encryptedSecrets.TmdbApiKey),
            Unprotect(encryptedSecrets.TmdbAccessToken),
            Unprotect(encryptedSecrets.PlexUsername),
            Unprotect(encryptedSecrets.PlexPassword));
    }

    public async Task WriteAsync(
        SecureAppSettings decryptedSecrets)
    {
        var encryptedSecrets = new SecureAppSettings(
            Protect(decryptedSecrets.OpenVPNUsername),
            Protect(decryptedSecrets.OpenVPNPassword),
            Protect(decryptedSecrets.TmdbApiKey),
            Protect(decryptedSecrets.TmdbAccessToken),
            Protect(decryptedSecrets.PlexUsername),
            Protect(decryptedSecrets.PlexPassword));
        var content = await Serializer.JsonSerializeAsync(encryptedSecrets);
        File.WriteAllText(_filePath, content);
    }

    #region Private use

    private string Protect(
        string input)
    {
        if (input == null)
        {
            return null;
        }

        return _dataProtector.Protect(input);
    }

    private string Unprotect(
        string input)
    {
        if (input == null)
        {
            return null;
        }

        return _dataProtector.Unprotect(input);
    }

    #endregion
}
