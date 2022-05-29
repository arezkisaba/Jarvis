namespace Jarvis.Features.Agents.TorrentClientAgent.Exceptions;

public class DemarrageServiceTorrentException : Exception
{
    public DemarrageServiceTorrentException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}