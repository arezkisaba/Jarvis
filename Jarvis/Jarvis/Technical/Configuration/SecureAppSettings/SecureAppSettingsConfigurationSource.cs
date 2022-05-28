using Jarvis.Technical.Configuration.SecureAppSettings.Services.Contracts;

namespace Jarvis.Technical.Configuration.SecureAppSettings;

public class SecureAppSettingsConfigurationSource : IConfigurationSource
{
    private readonly ISecureAppSettingsService _secureAppSettingsService;

    public SecureAppSettingsConfigurationSource(
        ISecureAppSettingsService secureAppSettingsService)
    {
        _secureAppSettingsService = secureAppSettingsService;
    }

    public IConfigurationProvider Build(
        IConfigurationBuilder builder)
    {
        return new SecureAppSettingsConfigurationProvider(_secureAppSettingsService);
    }
}