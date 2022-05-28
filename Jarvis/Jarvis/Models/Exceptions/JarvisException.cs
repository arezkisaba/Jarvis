namespace Jarvis.Models.Exceptions;

public class JarvisException : Exception
{
    public JarvisException(string message, Exception exception)
        : base(message, exception)
    {
    }
}
