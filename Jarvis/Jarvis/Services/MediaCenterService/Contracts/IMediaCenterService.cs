namespace Jarvis;

public interface IMediaCenterService
{
    Task<Tuple<IList<Movie>, IList<TvShow>>> FillMediasFromMediaCenterAndMediaDatabaseAsync();

    Task SynchronizeWatchStatusBetweenMediaCenterAndMediaDatabaseAsync();
}
