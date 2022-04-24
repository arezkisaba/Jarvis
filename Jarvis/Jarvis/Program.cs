using Jarvis;
using Lib.ApiServices.Plex;
using Lib.ApiServices.Tmdb;
using Lib.ApiServices.Transmission;
using Lib.Core;
using Lib.Win32;
using WindowsInput;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);

AppSettingsHelper.FillAppSettings(appSettings);

if (!Directory.Exists(appSettings.appdataDirectory))
{
    Directory.CreateDirectory(appSettings.appdataDirectory);
}

var tokensFile = StorageHelper.GetTokensFile(appSettings.appdataDirectory);
if (!File.Exists(tokensFile))
{
    var tokensModelSerialized = Serializer.JsonSerialize(new Tokens());
    File.Create(tokensFile).Close();
    File.WriteAllText(tokensFile, tokensModelSerialized);
}

var tokens = Serializer.JsonDeserialize<Tokens>(File.ReadAllText(tokensFile));

var secrets = new Secrets();
secrets.OpenVPNUsername = builder.Configuration["OpenVPN.Username"];
secrets.OpenVPNPassword = builder.Configuration["OpenVPN.Password"];
secrets.TmdbApiKey = builder.Configuration["Tmdb.ApiKey"];
secrets.TmdbAccessToken = builder.Configuration["Tmdb.AccessToken"];
secrets.PlexUsername = builder.Configuration["Plex.Username"];
secrets.PlexPassword = builder.Configuration["Plex.Password"];

builder.Services.AddSingleton(appSettings);
builder.Services.AddSingleton(secrets);
builder.Services.AddSingleton(tokens);
builder.Services.AddSingleton<IInputSimulator, InputSimulator>();
builder.Services.AddSingleton<IDisplayManager, DisplayManager>();
builder.Services.AddSingleton<IMailManager, MailManager>();
builder.Services.AddSingleton<INetworkManager, NetworkManager>();
builder.Services.AddSingleton<IPowerManager, PowerManager>();
builder.Services.AddSingleton<IProcessManager, ProcessManager>();
builder.Services.AddSingleton<IServiceManager, ServiceManager>();
builder.Services.AddSingleton<ISoundManager, SoundManager>();
builder.Services.AddSingleton<IWindowManager, WindowManager>();

builder.Services.AddSingleton<ITransmissionApiService>(new TransmissionApiService(
    appSettings.serviceConfig.transmissionConfig.url
));

builder.Services.AddSingleton<IPlexApiService>(new PlexApiService(
    appSettings.serviceConfig.plexConfig.url,
    secrets.PlexUsername,
    secrets.PlexPassword
));

builder.Services.AddSingleton<ITmdbApiService>(new TmdbApiService(
    secrets.TmdbApiKey,
    secrets.TmdbAccessToken,
    tokens.TmdbSessionId
));

builder.Services.AddSingleton<ICECService, CECService>();
builder.Services.AddSingleton<IIPResolverService, MonIPService>();
builder.Services.AddSingleton<IMediaStorageService, MediaStorageService>();
builder.Services.AddSingleton<IMediaCenterService, MediaCenterService>();
builder.Services.AddSingleton<IMediaService, MediaService>();
builder.Services.AddSingleton<ITorrentSearchService, TorrentSearchService>();
builder.Services.AddSingleton<ITorrentDownloaderService, TorrentDownloaderService>();
builder.Services.AddSingleton<IIPResolverBackgroundAgent, IPResolverBackgroundAgent>();
builder.Services.AddSingleton<IMediaStorageBackgroundAgent, MediaStorageBackgroundAgent>();
builder.Services.AddSingleton<IGameControllerClientBackgroundAgent, XboxControllerBackgroundAgent>();
builder.Services.AddSingleton<IVPNClientBackgroundAgent, OpenVPNBackgroundAgent>();
builder.Services.AddSingleton<ITorrentClientBackgroundAgent, TransmissionBackgroundAgent>();
builder.Services.AddSingleton<JarvisService>();

var app = builder.Build();

var jarvisService = app.Services.GetService<JarvisService>();
Task.Run(() => jarvisService.StartAsync());

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
