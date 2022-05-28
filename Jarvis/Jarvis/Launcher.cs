using Jarvis.BackgroundAgents.GameControllerBackgroundAgent;
using Jarvis.BackgroundAgents.GameControllerBackgroundAgent.Contracts;
using Jarvis.BackgroundAgents.GameControllerBackgroundAgent.Services.CECService;
using Jarvis.BackgroundAgents.GameControllerBackgroundAgent.Services.CECService.Contracts;
using Jarvis.BackgroundAgents.IPResolverBackgroundAgent;
using Jarvis.BackgroundAgents.IPResolverBackgroundAgent.Contracts;
using Jarvis.BackgroundAgents.MediaStorageBackgroundAgent;
using Jarvis.BackgroundAgents.MediaStorageBackgroundAgent.Contracts;
using Jarvis.BackgroundAgents.MediaStorageBackgroundAgent.MediaStorageService;
using Jarvis.BackgroundAgents.MediaStorageBackgroundAgent.MediaStorageService.Contracts;
using Jarvis.BackgroundAgents.TorrentClientBackgroundAgent;
using Jarvis.BackgroundAgents.TorrentClientBackgroundAgent.Contracts;
using Jarvis.BackgroundAgents.VPNClientBackgroundAgent;
using Jarvis.BackgroundAgents.VPNClientBackgroundAgent.Contracts;
using Jarvis.Configuration;
using Jarvis.Configuration.AppSettings.Models;
using Jarvis.Configuration.SecureAppSettings.Models;
using Jarvis.Configuration.SecureAppSettings.Services;
using Jarvis.Configuration.SecureAppSettings.Services.Contracts;
using Jarvis.Services;
using Jarvis.Services.MediaMatchingService;
using Jarvis.Services.MediaMatchingService.Contracts;
using Jarvis.Services.TorrentScrapperService;
using Jarvis.Services.TorrentScrapperService.Contracts;
using Jarvis.Services.TorrentScrapperService.Providers;
using Jarvis.Services.TorrentScrapperService.Providers.Bases;
using Jarvis.Shared.Components.Modaler.Services;
using Jarvis.Shared.Components.Toaster.Services;
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
        var appdataDirectory = configuration.GetValue<string>($"{nameof(AppSettingsModel)}:{nameof(AppSettingsModel.appdataDirectory)}");
        if (!Directory.Exists(appdataDirectory))
        {
            Directory.CreateDirectory(appdataDirectory);
        }

        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        services.Configure<AppSettingsModel>(configuration.GetSection("AppSettings"));
        services.Configure<SecureAppSettingsModel>(configuration.GetSection("SecureAppSettings"));

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
            var appSettings = serviceProvider.GetService<IOptions<AppSettingsModel>>();
            return new TransmissionApiService(
                appSettings.Value.serviceConfig.transmissionConfig.url);
        });

        services.AddSingleton<IPlexApiService>(serviceProvider =>
        {
            var appSettings = serviceProvider.GetService<IOptions<AppSettingsModel>>();
            var secureAppSettings = serviceProvider.GetService<IOptions<SecureAppSettingsModel>>();
            return new PlexApiService(
                appSettings.Value.serviceConfig.plexConfig.url,
                secureAppSettings.Value.PlexUsername,
                secureAppSettings.Value.PlexPassword);
        });

        services.AddSingleton<ITmdbApiService>(serviceProvider =>
        {
            var appSettings = serviceProvider.GetService<IOptions<AppSettingsModel>>();
            var secureAppSettings = serviceProvider.GetService<IOptions<SecureAppSettingsModel>>();
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

        services.AddScoped<ModalerService>();
        services.AddScoped<ToasterService>();

        services.AddSingleton<JarvisService>();
    }

    public Task StartAsync(
        IServiceProvider services)
    {
        var jarvisService = services.GetService<JarvisService>();
        return jarvisService.StartAsync();
    }
}
