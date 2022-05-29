namespace Jarvis.Features.Agents.TorrentClientAgent.Exceptions;

public class ArretServiceTorrentException : Exception
{
    public ArretServiceTorrentException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}