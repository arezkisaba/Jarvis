namespace Jarvis;

public interface ISecureAppSettingsService
{
    Task<SecureAppSettings> ReadAsync();

    Task WriteAsync(
        SecureAppSettings secureAppSettings);
}
