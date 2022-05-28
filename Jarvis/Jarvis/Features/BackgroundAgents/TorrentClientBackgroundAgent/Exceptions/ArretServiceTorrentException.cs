namespace Jarvis.Features.BackgroundAgents.TorrentClientBackgroundAgent.Exceptions;

public class ArretServiceTorrentException : Exception
{
    public ArretServiceTorrentException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}