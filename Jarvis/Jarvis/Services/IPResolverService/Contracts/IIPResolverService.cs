namespace Jarvis;

public interface IIPResolverService
{
    Task<string> GetAsync();
}
