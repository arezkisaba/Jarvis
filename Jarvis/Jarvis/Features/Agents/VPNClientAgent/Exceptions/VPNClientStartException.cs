namespace Jarvis.Features.Agents.VPNClientAgent.Exceptions;

[Serializable]
public class VPNClientStartException : Exception
{
    public VPNClientStartException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}