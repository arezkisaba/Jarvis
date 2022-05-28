using System.Runtime.Serialization;

namespace Jarvis.Features.BackgroundAgents.TorrentClientBackgroundAgent.Exceptions;

[Serializable]
internal class AjoutTorrentException : Exception
{
    public AjoutTorrentException()
    {
    }

    public AjoutTorrentException(
        string message)
        : base(message)
    {
    }

    public AjoutTorrentException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }

    protected AjoutTorrentException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
}