namespace Jarvis.Features.Agents.TorrentClientAgent.Exceptions;

public class TorrentClientStopException : Exception
{
    public TorrentClientStopException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}