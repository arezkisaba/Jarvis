namespace Jarvis.BackgroundAgents.IPResolverBackgroundAgent.IPResolverService.Contracts;

public interface IIPResolverService
{
    Task<string> GetAsync();
}
