namespace Jarvis;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddSecureAppSettingsConfiguration(
        this IConfigurationBuilder builder,
        ISecureAppSettingsService secureAppSettingsService)
    {
        return builder.Add(new SecureAppSettingsConfigurationSource(secureAppSettingsService));
    }
}
