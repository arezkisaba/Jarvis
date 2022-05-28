using Jarvis.Technical.Configuration.SecureAppSettings;
using Jarvis.Technical.Configuration.SecureAppSettings.Services.Contracts;

namespace Jarvis.Technical.Configuration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddSecureAppSettingsConfiguration(
        this IConfigurationBuilder builder,
        ISecureAppSettingsService secureAppSettingsService)
    {
        return builder.Add(new SecureAppSettingsConfigurationSource(secureAppSettingsService));
    }
}
