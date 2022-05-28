using System.Runtime.Serialization;

namespace Jarvis.Features.BackgroundAgents.VPNClientBackgroundAgent.Exceptions;

[Serializable]
internal class DemarrageServiceVPNException : Exception
{
    public DemarrageServiceVPNException()
    {
    }

    public DemarrageServiceVPNException(
        string message)
        : base(message)
    {
    }

    public DemarrageServiceVPNException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }

    protected DemarrageServiceVPNException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
}