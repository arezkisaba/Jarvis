using System.Runtime.Serialization;

namespace Jarvis.Features.BackgroundAgents.TorrentClientBackgroundAgent.Exceptions;

[Serializable]
internal class ArretServiceTorrentException : Exception
{
    public ArretServiceTorrentException()
    {
    }

    public ArretServiceTorrentException(
        string message)
        : base(message)
    {
    }

    public ArretServiceTorrentException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }

    protected ArretServiceTorrentException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
}