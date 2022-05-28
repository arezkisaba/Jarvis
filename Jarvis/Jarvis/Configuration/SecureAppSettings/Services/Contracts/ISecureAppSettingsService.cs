using Jarvis.Configuration.SecureAppSettings.Models;

namespace Jarvis.Configuration.SecureAppSettings.Services.Contracts;

public interface ISecureAppSettingsService
{
    Task<SecureAppSettingsModel> ReadAsync();

    Task WriteAsync(
        SecureAppSettingsModel secureAppSettings);
}
