namespace Jarvis.Features.Agents.VPNClientAgent.Exceptions;

[Serializable]
public class VPNClientStopException : Exception
{
    public VPNClientStopException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}