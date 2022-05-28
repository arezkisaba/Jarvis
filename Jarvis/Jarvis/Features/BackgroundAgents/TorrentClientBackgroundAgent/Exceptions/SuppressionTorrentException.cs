using System.Runtime.Serialization;

namespace Jarvis.Features.BackgroundAgents.TorrentClientBackgroundAgent.Exceptions;

[Serializable]
internal class SuppressionTorrentException : Exception
{
    public SuppressionTorrentException()
    {
    }

    public SuppressionTorrentException(
        string message)
        : base(message)
    {
    }

    public SuppressionTorrentException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }

    protected SuppressionTorrentException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
}