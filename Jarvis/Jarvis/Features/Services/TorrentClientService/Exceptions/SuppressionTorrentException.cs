namespace Jarvis.Features.Services.TorrentClientService.Exceptions;

public class SuppressionTorrentException : Exception
{
    public SuppressionTorrentException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}