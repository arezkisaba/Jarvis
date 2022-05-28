using Jarvis.Technical.Configuration.SecureAppSettings.Models;

namespace Jarvis.Technical.Configuration.SecureAppSettings.Services.Contracts;

public interface ISecureAppSettingsService
{
    Task<SecureAppSettingsModel> ReadAsync();

    Task WriteAsync(
        SecureAppSettingsModel secureAppSettings);
}
