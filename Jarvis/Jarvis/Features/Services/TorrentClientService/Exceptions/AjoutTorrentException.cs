namespace Jarvis.Features.Services.TorrentClientService.Exceptions;

public class AjoutTorrentException : Exception
{
    public AjoutTorrentException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}