namespace Jarvis.Features.Agents.VPNClientAgent.Exceptions;

[Serializable]
public class ArretServiceVPNException : Exception
{
    public ArretServiceVPNException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}