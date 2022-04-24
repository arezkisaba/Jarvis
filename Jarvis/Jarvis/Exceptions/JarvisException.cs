namespace Jarvis;

public class JarvisException : Exception
{
    public JarvisException(string message, Exception exception)
        : base(message, exception)
    {
    }
}
