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
        IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        services.AddSingleton<WeatherForecastService>();

        var appSettings = new AppSettings();
        configurationManager.Bind(appSettings);

        AppSettingsHelper.FillAppSettings(appSettings);

        if (!Directory.Exists(appSettings.appdataDirectory))
        {
            Directory.CreateDirectory(appSettings.appdataDirectory);
        }

        var tokensFile = StorageHelper.GetTokensFile(appSettings.appdataDirectory);
        if (!File.Exists(tokensFile))
        {
            File.Create(tokensFile).Close();
        }

        var secureAppSettingsFile = StorageHelper.GetSecureAppSettingsFile(appSettings.appdataDirectory);
        if (!File.Exists(secureAppSettingsFile))
        {
            File.Create(secureAppSettingsFile).Close();
        }

        var tokens = Serializer.JsonDeserialize<Tokens>(File.ReadAllText(tokensFile));

        var secureAppSettingsService = new SecureAppSettingsService(secureAppSettingsFile);
        var secureAppSettings = await secureAppSettingsService.ReadAsync();

        services.AddSingleton(appSettings);
        services.AddSingleton(secureAppSettings);
        services.AddSingleton(tokens);
        services.AddSingleton<ISecureAppSettingsService>(secureAppSettingsService);
        services.AddSingleton<IInputSimulator, InputSimulator>();
        services.AddSingleton<IDisplayManager, DisplayManager>();
        services.AddSingleton<IMailManager, MailManager>();
        services.AddSingleton<INetworkManager, NetworkManager>();
        services.AddSingleton<IPowerManager, PowerManager>();
        services.AddSingleton<IProcessManager, ProcessManager>();
        services.AddSingleton<IServiceManager, ServiceManager>();
        services.AddSingleton<ISoundManager, SoundManager>();
        services.AddSingleton<IWindowManager, WindowManager>();

        services.AddSingleton<ITransmissionApiService>(new TransmissionApiService(
            appSettings.serviceConfig.transmissionConfig.url
        ));

        services.AddSingleton<IPlexApiService>(new PlexApiService(
            appSettings.serviceConfig.plexConfig.url,
            secureAppSettings.PlexUsername,
            secureAppSettings.PlexPassword
        ));

        services.AddSingleton<ITmdbApiService>(new TmdbApiService(
            secureAppSettings.TmdbApiKey,
            secureAppSettings.TmdbAccessToken,
            tokens.TmdbSessionId
        ));

        services.AddSingleton<ICECService, CECService>();
        ////services.AddSingleton<IIPResolverService, WhatIsMyPublicIPService>();
        services.AddSingleton<IMediaStorageService, MediaStorageService>();
        services.AddSingleton<IMediaCenterService, MediaCenterService>();
        services.AddSingleton<IMediaService, MediaService>();
        services.AddSingleton<ITorrentSearchService, TorrentSearchService>();
        services.AddSingleton<ITorrentDownloaderService, TorrentDownloaderService>();
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
