using Jarvis.Configuration.SecureAppSettings;
using Jarvis.Configuration.SecureAppSettings.Services.Contracts;

namespace Jarvis.Configuration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddSecureAppSettingsConfiguration(
        this IConfigurationBuilder builder,
        ISecureAppSettingsService secureAppSettingsService)
    {
        return builder.Add(new SecureAppSettingsConfigurationSource(secureAppSettingsService));
    }
}
