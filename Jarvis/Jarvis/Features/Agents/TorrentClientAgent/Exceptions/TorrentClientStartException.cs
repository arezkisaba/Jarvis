namespace Jarvis.Features.Agents.TorrentClientAgent.Exceptions;

public class TorrentClientStartException : Exception
{
    public TorrentClientStartException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}