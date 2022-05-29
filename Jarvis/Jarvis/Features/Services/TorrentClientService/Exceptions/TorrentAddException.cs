namespace Jarvis.Features.Services.TorrentClientService.Exceptions;

public class TorrentAddException : Exception
{
    public TorrentAddException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}