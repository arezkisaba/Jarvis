using System.Runtime.Serialization;

namespace Jarvis.Features.BackgroundAgents.VPNClientBackgroundAgent.Exceptions;

[Serializable]
public class DemarrageServiceVPNException : Exception
{
    public DemarrageServiceVPNException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}