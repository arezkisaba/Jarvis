namespace Jarvis.Features.BackgroundAgents.TorrentClientBackgroundAgent.Exceptions;

public class DemarrageServiceTorrentException : Exception
{
    public DemarrageServiceTorrentException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}