namespace Jarvis.Features.Services.TorrentClientService.Exceptions;

public class TorrentDeleteException : Exception
{
    public TorrentDeleteException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}