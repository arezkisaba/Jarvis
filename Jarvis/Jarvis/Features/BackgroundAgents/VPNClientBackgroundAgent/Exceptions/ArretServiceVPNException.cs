using System.Runtime.Serialization;

namespace Jarvis.Features.BackgroundAgents.VPNClientBackgroundAgent.Exceptions;

[Serializable]
internal class ArretServiceVPNException : Exception
{
    public ArretServiceVPNException()
    {
    }

    public ArretServiceVPNException(
        string message) : base(message)
    {
    }

    public ArretServiceVPNException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }

    protected ArretServiceVPNException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
}