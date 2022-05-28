using Jarvis.Technical.Configuration.SecureAppSettings.Services.Contracts;

namespace Jarvis.Technical.Configuration.SecureAppSettings;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddSecureAppSettingsConfiguration(
        this IConfigurationBuilder builder,
        ISecureAppSettingsService secureAppSettingsService)
    {
        return builder.Add(new SecureAppSettingsConfigurationSource(secureAppSettingsService));
    }
}
