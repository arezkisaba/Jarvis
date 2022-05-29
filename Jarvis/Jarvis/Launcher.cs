using Jarvis.Features.Agents.GameControllerAgent;
using Jarvis.Features.Agents.GameControllerAgent.Contracts;
using Jarvis.Features.Agents.GameControllerAgent.Services.CECService;
using Jarvis.Features.Agents.GameControllerAgent.Services.CECService.Contracts;
using Jarvis.Features.Agents.IPResolverAgent;
using Jarvis.Features.Agents.IPResolverAgent.Contracts;
using Jarvis.Features.Agents.MediaStorageAgent;
using Jarvis.Features.Agents.MediaStorageAgent.Contracts;
using Jarvis.Features.Agents.MediaStorageAgent.Services.MediaStorageService;
using Jarvis.Features.Agents.MediaStorageAgent.Services.MediaStorageService.Contracts;
using Jarvis.Features.Agents.TorrentClientAgent;
using Jarvis.Features.Agents.TorrentClientAgent.Contracts;
using Jarvis.Features.Agents.VPNClientAgent;
using Jarvis.Features.Agents.VPNClientAgent.Contracts;
using Jarvis.Features.Services;
using Jarvis.Features.Services.MediaMatchingService;
using Jarvis.Features.Services.MediaMatchingService.Contracts;
using Jarvis.Features.Services.MediaNamingService;
using Jarvis.Features.Services.MediaNamingService.Contracts;
using Jarvis.Features.Services.MediaRenamerService;
using Jarvis.Features.Services.MediaRenamerService.Contracts;
using Jarvis.Features.Services.TorrentClientService;
using Jarvis.Features.Services.TorrentClientService.Contracts;
using Jarvis.Features.Services.TorrentScrapperService;
using Jarvis.Features.Services.TorrentScrapperService.Contracts;
using Jarvis.Features.Services.TorrentScrapperService.Providers;
using Jarvis.Features.Services.TorrentScrapperService.Providers.Bases;
using Jarvis.Shared.Components.Modaler.Services;
using Jarvis.Shared.Components.Toaster.Services;
using Jarvis.Technical.Configuration.AppSettings.Models;
using Jarvis.Technical.Configuration.SecureAppSettings;
using Jarvis.Technical.Configuration.SecureAppSettings.Models;
using Jarvis.Technical.Configuration.SecureAppSettings.Services;
using Jarvis.Technical.Configuration.SecureAppSettings.Services.Contracts;
using Lib.ApiServices.Plex;
using Lib.ApiServices.Tmdb;
using Lib.ApiServices.Transmission;
using Lib.Core;
using Lib.Win32;
using WindowsInput;

namespace Jarvis;

public class Launcher
{
    public async Task InitAsync(
        ConfigurationManager configuration,
        IServiceCollection services)
    {
        var appdataDirectory = configuration.GetValue<string>($"AppSettings:{nameof(AppSettingsModel.appdataDirectory)}");
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

        services.AddScoped<IMediaNamingService, MediaNamingService>();
        services.AddScoped<IMediaMatchingService, MediaMatchingService>();
        services.AddScoped<IMediaRenamerService, MediaRenamerService>();

        services.AddSingleton<ICECService, CECService>();
        services.AddSingleton<IMediaStorageService, MediaStorageService>();
        services.AddSingleton<ITorrentClientService, TorrentClientService>();

        services.AddSingleton<IIPResolverAgent, IPResolverAgent>();
        services.AddSingleton<IMediaStorageAgent, MediaStorageAgent>();
        services.AddSingleton<IGameControllerClientAgent, XboxControllerAgent>();
        services.AddSingleton<IVPNClientAgent, OpenVPNAgent>();
        services.AddSingleton<ITorrentClientAgent, TransmissionAgent>();

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
