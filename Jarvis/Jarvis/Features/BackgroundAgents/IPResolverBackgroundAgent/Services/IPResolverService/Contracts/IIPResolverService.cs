namespace Jarvis.Features.BackgroundAgents.IPResolverBackgroundAgent.Services.IPResolverService.Contracts;

public interface IIPResolverService
{
    Task<string> GetAsync();
}
