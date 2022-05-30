using Jarvis.Features.Agents.TorrentClientAgent.Contracts;
using Jarvis.Features.Services.MediaMatchingService.Contracts;
using Jarvis.Features.Services.MediaMatchingService.Models;
using Jarvis.Features.Services.MediaNamingService.Contracts;
using Jarvis.Features.Services.MediaRenamerService.Contracts;
using Jarvis.Features.Services.TorrentClientService.Contracts;
using Jarvis.Features.Services.TorrentClientService.Models;
using Jarvis.Shared.Components.Modaler.Services;
using Jarvis.Shared.Components.Toaster.Models;
using Jarvis.Shared.Components.Toaster.Services;
using Jarvis.Technical;
using Jarvis.Technical.Configuration.AppSettings.Models;
using Jarvis.Technical.Configuration.SecureAppSettings.Services.Contracts;
using Lib.ApiServices.Tmdb;
using Lib.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Jarvis.Shared;

public partial class MainLayout : BlazorLayoutComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Inject]
    private IOptions<AppSettingsModel> AppSettings { get; set; }

    [Inject]
    private IStringLocalizer<App> AppLocalizer { get; set; }

    [Inject]
    private IStringLocalizer<MainLayout> Localizer { get; set; }

    [Inject]
    private ISecureAppSettingsService SecureAppSettingsService { get; set; }

    [Inject]
    private ITorrentClientAgent TorrentClientAgent { get; set; }

    [Inject]
    private ITorrentClientService TorrentClientService { get; set; }

    [Inject]
    private IMediaNamingService MediaNamingService { get; set; }

    [Inject]
    private IMediaMatchingService MediaMatchingService { get; set; }

    [Inject]
    private IMediaRenamerService MediaRenamerService { get; set; }

    [Inject]
    private ITmdbApiService TmdbApiService { get; set; }

    [Inject]
    private ModalerService ModalerService { get; set; }

    [Inject]
    private ToasterService ToasterService { get; set; }

    private bool ShowAlert { get; set; }

    private string TmdbAuthenticationUrl { get; set; }

    private string AppName { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        AppName = AppSettings.Value.appName;

        SetCultureInfo();

        TorrentClientAgent.DownloadFinishedAction = async (download) =>
        {
            await HandleDownloadFinishedAsync(download);
        };

        TmdbApiService.AuthenticationInformationsAvailable += async (sender, e) =>
        {
            TmdbAuthenticationUrl = e.AuthenticationUrl;
            ////await UpdateUIAsync();
        };

        TmdbApiService.AuthenticationSuccessfull += async (sender, e) =>
        {
            try
            {
                var secureAppSettings = await SecureAppSettingsService.ReadAsync();
                secureAppSettings.TmdbSessionId = e.SessionId;
                await SecureAppSettingsService.WriteAsync(secureAppSettings);
            }
            catch (Exception)
            {
                // IGNORE
            }
            finally
            {
            }
        };

        Task.Run(async () =>
        {
            ////await Task.Delay(2000);
            ////ToasterService.AddToast(Toast.CreateToast("Hello World 1", "Hello from Blazor", ToastType.Success, 5));
            ////await Task.Delay(2000);
            ////ToasterService.AddToast(Toast.CreateToast("Hello World 2", "Hello from Blazor", ToastType.Danger, 5));
            ////await Task.Delay(2000);
            ////ModalerService.AddModal(Modal.CreateModal("Hello World 1", "Hello from Blazor", ModalType.Warning));
        });
    }

    #region Private use

    private async Task HandleDownloadFinishedAsync(
        TorrentDownloadModel download)
    {
        try
        {
            var (mediaType, match) = MediaMatchingService.GetMediaTypeAndInformations(download.Name);
            if (mediaType == MediaTypeModel.Episode)
            {
                var torrentTitles = MediaNamingService.GetPossibleMediaTitles(match.Groups[1].Value);
                foreach (var torrentTitle in torrentTitles)
                {
                    var seasonNumber = int.Parse(match.Groups[2].Value);
                    var episodeNumber = int.Parse(match.Groups[3].Value);
                    var tvShows = await TmdbApiService.SearchTvShowAsync(torrentTitle);
                    if (tvShows.Any())
                    {
                        var tvShow = tvShows.ElementAt(0);
                        var seasons = await TmdbApiService.GetSeasonsAsync(tvShow.Id);
                        var season = seasons.SingleOrDefault(obj => obj.Number == seasonNumber);
                        if (season != null)
                        {
                            await MediaRenamerService.RenameEpisodeAsync(download.DownloadDirectory, tvShow.Title.TransformForStorage(), seasonNumber, episodeNumber, "FRENCH");
                            var message = string.Format(Localizer["Toaster.DownloadEnded"], $"{MediaNamingService.GetDisplayNameForEpisode(tvShow.Title, seasonNumber, episodeNumber)}");
                            ToasterService.AddToast(Toast.CreateToast(AppLocalizer["Toaster.InformationTitle"], message, ToastType.Success, 2));
                            break;
                        }
                    }
                }
            }
            else if (mediaType == MediaTypeModel.Season)
            {
                var torrentTitles = MediaNamingService.GetPossibleMediaTitles(match.Groups[1].Value);
                foreach (var torrentTitle in torrentTitles)
                {
                    var seasonNumber = int.Parse(match.Groups[2].Value);
                    var tvShows = await TmdbApiService.SearchTvShowAsync(torrentTitle);
                    if (tvShows.Any())
                    {
                        var tvShow = tvShows.ElementAt(0);
                        var seasons = await TmdbApiService.GetSeasonsAsync(tvShow.Id);
                        var season = seasons.SingleOrDefault(obj => obj.Number == seasonNumber);
                        if (season != null)
                        {
                            var episodes = await TmdbApiService.GetEpisodesAsync(tvShow.Id, seasonNumber);
                            for (var i = 1; i <= episodes.Count(); i++)
                            {
                                await MediaRenamerService.RenameEpisodeAsync(download.DownloadDirectory, tvShow.Title.TransformForStorage(), seasonNumber, i, "FRENCH");
                            }

                            var message = string.Format(Localizer["Toaster.DownloadEnded"], $"{MediaNamingService.GetDisplayNameForSeason(tvShow.Title, seasonNumber)}");
                            ToasterService.AddToast(Toast.CreateToast(AppLocalizer["Toaster.InformationTitle"], message, ToastType.Success, 2));
                            break;
                        }
                    }
                }
            }
            else if (mediaType == MediaTypeModel.Movie)
            {
                var torrentTitles = MediaNamingService.GetPossibleMediaTitles(match.Groups[1].Value);
                foreach (var torrentTitle in torrentTitles)
                {
                    var movies = await TmdbApiService.SearchMovieAsync(torrentTitle);
                    if (movies.Any())
                    {
                        var movie = movies.ElementAt(0);
                        await MediaRenamerService.RenameMovieAsync(download.DownloadDirectory, movie.Title.TransformForStorage(), movie.Year);
                        var message = string.Format(Localizer["Toaster.DownloadEnded"], $"{MediaNamingService.GetDisplayNameForMovie(movie.Title)}");
                        ToasterService.AddToast(Toast.CreateToast(AppLocalizer["Toaster.InformationTitle"], message, ToastType.Success, 2));
                        break;
                    }
                }
            }

            await TorrentClientService.DeleteDownloadAsync(download.HashString);
        }
        catch (Exception)
        {
            ToasterService.AddToast(Toast.CreateToast(AppLocalizer["Toaster.ErrorTitle"], AppLocalizer["Toaster.ErrorMessage"], ToastType.Danger, 2));
        }
    }

    private void SetCultureInfo()
    {
        if (QueryHelpers.ParseQuery(new Uri(NavigationManager.Uri).Query).TryGetValue("lang", out var paramLang))
        {
            CultureInfo culture;

            try
            {
                culture = new CultureInfo(paramLang);
            }
            catch
            {
                culture = new CultureInfo("en");
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }

    #endregion
}
