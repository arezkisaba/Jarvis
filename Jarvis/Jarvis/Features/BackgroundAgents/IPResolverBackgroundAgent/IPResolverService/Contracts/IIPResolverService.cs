namespace Jarvis.Features.BackgroundAgents.IPResolverBackgroundAgent.IPResolverService.Contracts;

public interface IIPResolverService
{
    Task<string> GetAsync();
}
