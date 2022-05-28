using System.Runtime.Serialization;

namespace Jarvis.Features.BackgroundAgents.TorrentClientBackgroundAgent.Exceptions;

[Serializable]
internal class DemarrageServiceTorrentException : Exception
{
    public DemarrageServiceTorrentException()
    {
    }

    public DemarrageServiceTorrentException(
        string message)
        : base(message)
    {
    }

    public DemarrageServiceTorrentException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }

    protected DemarrageServiceTorrentException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
}