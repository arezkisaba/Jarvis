using Microsoft.Extensions.Options;

namespace Jarvis;

public class ApplicationStorageService : IApplicationStorageService
{
    private readonly AppSettings _appSettings;
    private readonly Tokens _tokens;
    private readonly ILogger<MediaCenterService> _logger;

    public ApplicationStorageService(
        IOptions<AppSettings> appSettings,
        Tokens tokens,
        ILogger<MediaCenterService> logger)
    {
        _appSettings = appSettings.Value;
        _tokens = tokens;
        _logger = logger;
    }
}
