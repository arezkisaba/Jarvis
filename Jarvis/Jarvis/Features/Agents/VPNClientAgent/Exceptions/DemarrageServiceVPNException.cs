namespace Jarvis.Features.Agents.VPNClientAgent.Exceptions;

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