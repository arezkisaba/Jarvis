using Lib.ApiServices.Plex;
using Lib.ApiServices.Tmdb;
using Lib.ApiServices.Transmission;
using Lib.Core;
using Lib.Win32;
using Microsoft.Extensions.Options;
using WindowsInput;

namespace Jarvis;

public class Launcher
{
    public async Task InitAsync(
        ConfigurationManager configuration,
        IServiceCollection services)
    {
        var appdataDirectory = configuration.GetValue<string>($"{nameof(AppSettings)}:{nameof(AppSettings.appdataDirectory)}");
        if (!Directory.Exists(appdataDirectory))
        {
            Directory.CreateDirectory(appdataDirectory);
        }

        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.Configure<SecureAppSettings>(configuration.GetSection("SecureAppSettings"));

        var secureAppSettingsService = new SecureAppSettingsService(appdataDirectory);
        services.AddSingleton<ISecureAppSettingsService>(secureAppSettingsService);
        configuration.AddSecureAppSettingsConfiguration(secureAppSettingsService);

        services.AddSingleton<IInputSimulator, InputSimulator>();
        services.AddSingleton<IDisplayManager, DisplayManager>();
        services.AddSingleton<IMailManager, MailManager>();
        services.AddSingleton<INetworkManager, NetworkManager>();
        services.AddSingleton<IPowerManager, PowerManager>();
        services.AddSingleton<IProcessManager, ProcessManager>();
        services.AddSingleton<IServiceManager, ServiceManager>();
        services.AddSingleton<ISoundManager, SoundManager>();
        services.AddSingleton<IWindowManager, WindowManager>();

        services.AddSingleton<ITransmissionApiService>(serviceProvider =>
        {
            var appSettings = serviceProvider.GetService<IOptions<AppSettings>>();
            return new TransmissionApiService(
                appSettings.Value.serviceConfig.transmissionConfig.url);
        });

        services.AddSingleton<IPlexApiService>(serviceProvider =>
        {
            var appSettings = serviceProvider.GetService<IOptions<AppSettings>>();
            var secureAppSettings = serviceProvider.GetService<IOptions<SecureAppSettings>>();
            return new PlexApiService(
                appSettings.Value.serviceConfig.plexConfig.url,
                secureAppSettings.Value.PlexUsername,
                secureAppSettings.Value.PlexPassword);
        });

        services.AddSingleton<ITmdbApiService>(serviceProvider =>
        {
            var appSettings = serviceProvider.GetService<IOptions<AppSettings>>();
            var secureAppSettings = serviceProvider.GetService<IOptions<SecureAppSettings>>();
            return new TmdbApiService(
                secureAppSettings.Value.TmdbApiKey,
                secureAppSettings.Value.TmdbAccessToken,
                secureAppSettings.Value.TmdbSessionId
            );
        });

        services.AddSingleton<ITorrentScrapperService>(new TorrentScrapperService(
            new List<TorrentScrapperServiceBase>
            {
                new GkTorrentTorrentScrapperService("https://www.gktorrents.org"),
                new OxTorrentTorrentScrapperService("https://www.oxtorrent.si"),
                new Torrent9TorrentScrapperService("https://www.torrent9.nl"),
                new ZeTorrentsTorrentScrapperService("https://www.zetorrents.nl"),
            }
        ));

        services.AddSingleton<ICECService, CECService>();
        services.AddSingleton<IMediaMatchingService, MediaMatchingService>();
        services.AddSingleton<IMediaStorageService, MediaStorageService>();
        ////services.AddSingleton<IMediaCenterService, MediaCenterService>();
        ////services.AddSingleton<IMediaService, MediaService>();
        services.AddSingleton<IIPResolverBackgroundAgent, IPResolverBackgroundAgent>();
        services.AddSingleton<IMediaStorageBackgroundAgent, MediaStorageBackgroundAgent>();
        services.AddSingleton<IGameControllerClientBackgroundAgent, XboxControllerBackgroundAgent>();
        services.AddSingleton<IVPNClientBackgroundAgent, OpenVPNBackgroundAgent>();
        services.AddSingleton<ITorrentClientBackgroundAgent, TransmissionBackgroundAgent>();
        services.AddSingleton<JarvisService>();
    }

    public Task StartAsync(
        IServiceProvider services)
    {
        var jarvisService = services.GetService<JarvisService>();
        return jarvisService.StartAsync();
    }
}
